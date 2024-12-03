#!/bin/bash
cd ../
cd ../
mkdir -p tests/StyleCop
for file in $(find . -type f -name "*.cs"); do
    filename=$(basename "$file" .cs)
    output_file="tests/StyleCop/${filename}_stylecop.txt"
    dotnet build $file --no-restore --warnaserror > "$output_file"
    echo "$file -> $output_file" >> "$output_file"
done
