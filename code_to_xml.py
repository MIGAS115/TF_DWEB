#!/usr/bin/env python3
"""
code_to_xml.py
Converts a source code directory into a highly compressed, AI-ready XML file.
Tailored for ASP.NET Core projects (Razor Pages + MVC, C#).
"""

import os
import sys
import re
import io
from pathlib import Path

# ─────────────────────────────────────────────
#  OUTPUT CONFIGURATION
# ─────────────────────────────────────────────
OUTPUT_FILE = "project_snapshot.xml"
TARGET_DIR  = sys.argv[1] if len(sys.argv) > 1 else "."

# ─────────────────────────────────────────────
#  COMPRESSION OPTIONS
# ─────────────────────────────────────────────
HIDE_COMMENTS      = True
REMOVE_BLANK_LINES = True
TRIM_TRAILING      = True

# ─────────────────────────────────────────────
#  INCLUDED EXTENSIONS
# ─────────────────────────────────────────────
INCLUDED_EXTENSIONS = {
    ".cs", ".cshtml", ".razor", ".csproj", ".sln", ".json", ".xml",
    ".html", ".htm", ".css", ".js", ".ts", ".txt", ".md", ".env", ".yaml", ".yml",
}

# ─────────────────────────────────────────────
#  EXCLUSION FILTERS
# ─────────────────────────────────────────────
EXCLUDED_PATH_SEGMENTS = {
    "bin", "obj", "node_modules", "packages", ".nuget",
    ".git", ".vs", ".vscode", ".idea", "lib", "Migrations",
}

# Exact relative folder paths to ignore (e.g., wwwroot/lib for LibMan)
EXCLUDED_PATH_PREFIXES = {
    "wwwroot/lib", "wwwroot/node_modules", "wwwroot/dist"
}

EXCLUDED_FILENAMES = {
    ".gitignore", ".gitattributes", ".editorconfig", ".dockerignore",
    "thumbs.db", "desktop.ini", "package-lock.json", "yarn.lock",
    "bundleconfig.json", "libman.json",
}

# Suffixes for minified, auto-generated, or map files
EXCLUDED_SUFFIXES = {
    ".min.js", ".min.css", ".min.map", ".map",
    ".g.cs", ".g.i.cs", ".Designer.cs",
}

# ─────────────────────────────────────────────
#  PRE-COMPILED REGEX (Performance Boost)
# ─────────────────────────────────────────────
RE_BLOCK_COMMENT   = re.compile(r"/\*.*?\*/", re.DOTALL)
RE_SINGLE_COMMENT  = re.compile(r"(?<![:])//.*") # Avoids stripping http://
RE_HTML_COMMENT    = re.compile(r"<!--.*?-->", re.DOTALL)
RE_RAZOR_COMMENT   = re.compile(r"@\*.*?\*@", re.DOTALL)

# ─────────────────────────────────────────────
#  COMMENT-STRIPPING HELPERS
# ─────────────────────────────────────────────
def process_content(content: str, ext: str) -> str:
    if HIDE_COMMENTS:
        if ext in {".cs", ".js", ".ts"}:
            content = RE_BLOCK_COMMENT.sub("", content)
            content = RE_SINGLE_COMMENT.sub("", content)
        elif ext in {".cshtml", ".razor"}:
            content = RE_RAZOR_COMMENT.sub("", content)
            content = RE_HTML_COMMENT.sub("", content)
            content = RE_BLOCK_COMMENT.sub("", content)
            content = RE_SINGLE_COMMENT.sub("", content)
        elif ext in {".html", ".htm"}:
            content = RE_HTML_COMMENT.sub("", content)
        elif ext in {".css"}:
            content = RE_BLOCK_COMMENT.sub("", content)

    lines = content.splitlines()

    if TRIM_TRAILING:
        lines = [l.rstrip() for l in lines]

    if REMOVE_BLANK_LINES:
        lines = [l for l in lines if l.strip()]

    return "\n".join(lines)

# ─────────────────────────────────────────────
#  PATH FILTERING
# ─────────────────────────────────────────────
def is_excluded(path: Path, root: Path) -> bool:
    try:
        rel = path.relative_to(root)
    except ValueError:
        rel = path

    if path.suffix.lower() in EXCLUDED_SUFFIXES:
        return True
    if path.name.lower() in {f.lower() for f in EXCLUDED_FILENAMES}:
        return True
    if path.suffix.lower() not in INCLUDED_EXTENSIONS:
        return True
        
    return False

# ─────────────────────────────────────────────
#  DIRECTORY TREE GENERATION
# ─────────────────────────────────────────────
def get_tree_lines(dir_path: Path, prefix: str = "", is_last: bool = True, root_path: Path = None) -> list[str]:
    if root_path is None:
        root_path = dir_path

    lines = []
    if dir_path == root_path:
        lines.append(f"{dir_path.resolve().name}/")
        prefix = ""
    else:
        connector = "└" if is_last else "├"
        lines.append(f"{prefix}{connector}── {dir_path.name}/")
        prefix += "    " if is_last else "│   "

    children = []
    try:
        for item in dir_path.iterdir():
            if item.is_dir():
                rel = item.relative_to(root_path)
                if item.name in EXCLUDED_PATH_SEGMENTS or item.name.startswith(".") or str(rel) in EXCLUDED_PATH_PREFIXES:
                    continue
                children.append(item)
            else:
                if is_excluded(item, root_path):
                    continue
                children.append(item)
    except PermissionError:
        pass

    children.sort(key=lambda x: (not x.is_dir(), x.name.lower()))

    for i, child in enumerate(children):
        child_is_last = (i == len(children) - 1)
        connector = "└" if child_is_last else "├"
        
        if child.is_dir():
            lines.extend(get_tree_lines(child, prefix, child_is_last, root_path))
        else:
            lines.append(f"{prefix}{connector}── {child.name}")

    return lines

# ─────────────────────────────────────────────
#  FILE SCANNER
# ─────────────────────────────────────────────
def scan_project(root_dir: Path) -> tuple[list[str], list[tuple[str, str, str]]]:
    print("  Generating directory structure...")
    tree_lines = get_tree_lines(root_dir)
    
    files_data = []
    print("  Scanning files...")
    
    for dirpath, dirnames, filenames in os.walk(root_dir):
        current = Path(dirpath)
        rel_current = current.relative_to(root_dir)

        # Prune directories in-place so os.walk doesn't descend into them
        dirnames[:] = [
            d for d in dirnames
            if d not in EXCLUDED_PATH_SEGMENTS 
            and not d.startswith(".") 
            and str(rel_current / d) not in EXCLUDED_PATH_PREFIXES
        ]
        dirnames.sort()

        for filename in sorted(filenames):
            filepath = current / filename

            if is_excluded(filepath, root_dir):
                continue

            rel_path = str(filepath.relative_to(root_dir)).replace("\\", "/")

            try:
                raw = filepath.read_text(encoding="utf-8", errors="replace")
            except Exception as exc:
                print(f"  [WARN] Could not read {rel_path}: {exc}")
                continue

            processed = process_content(raw, filepath.suffix.lower())
            files_data.append((rel_path, filepath.suffix.lower(), processed))
            print(f"  + {rel_path}")

    return tree_lines, files_data

# ─────────────────────────────────────────────
#  FAST XML WRITER (Memory Efficient)
# ─────────────────────────────────────────────
def write_xml(root_dir: Path, tree_lines: list[str], files_data: list[tuple], output_path: Path) -> None:
    # Using StringIO is exponentially faster and uses less memory than ElementTree/minidom
    with io.StringIO() as buf:
        buf.write('<?xml version="1.0" encoding="utf-8"?>\n')
        buf.write(f'<Project root="{root_dir.resolve()}" totalFiles="{len(files_data)}">\n')
        
        buf.write('  <Options>\n')
        buf.write(f'    <HideComments>{HIDE_COMMENTS}</HideComments>\n')
        buf.write(f'    <RemoveBlankLines>{REMOVE_BLANK_LINES}</RemoveBlankLines>\n')
        buf.write(f'    <TrimTrailing>{TRIM_TRAILING}</TrimTrailing>\n')
        buf.write('  </Options>\n')
        
        buf.write('  <DirectoryStructure>\n')
        buf.write('\n'.join(tree_lines))
        buf.write('\n  </DirectoryStructure>\n')
        
        # xml:space="preserve" ensures AI parsers don't strip intentional indentation
        buf.write('  <Files xml:space="preserve">\n')
        for rel_path, ext, content in files_data:
            # Protect against rare CDATA ending collisions inside the code itself
            safe_content = content.replace("]]>", "]]]]><![CDATA[>")
            buf.write(f'    <File path="{rel_path}" extension="{ext}"><![CDATA[{safe_content}]]></File>\n')
        buf.write('  </Files>\n')
        
        buf.write('</Project>\n')
        
        # Write buffer straight to disk
        output_path.write_text(buf.getvalue(), encoding="utf-8")

# ─────────────────────────────────────────────
#  MAIN
# ─────────────────────────────────────────────
def main() -> None:
    root_dir = Path(TARGET_DIR).resolve()

    if not root_dir.is_dir():
        print(f"[ERROR] Target directory not found: {root_dir}")
        return

    output_path = root_dir / OUTPUT_FILE

    print(f"\n{'='*60}")
    print(f"  ASP.NET Core Code to XML Compiler (Optimized)")
    print(f"  Source : {root_dir}")
    print(f"  Output : {output_path}")
    print(f"  Options: comments={'hidden' if HIDE_COMMENTS else 'kept'} | "
          f"blank_lines={'removed' if REMOVE_BLANK_LINES else 'kept'}")
    print(f"{'='*60}\n")

    tree_lines, files_data = scan_project(root_dir)
    write_xml(root_dir, tree_lines, files_data, output_path)

    print(f"\n{'='*60}")
    print(f"  Done! {len(files_data)} file(s) written to: {output_path}")
    print(f"{'='*60}\n")

if __name__ == "__main__":
    main()