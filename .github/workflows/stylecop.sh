#!/bin/bash
cd ../
cd ../
mkdir -p tests/StyleCop

# Loop through all C# files and run StyleCop checks
for file in $(find . -type f -name "*.cs"); do
    filename=$(basename "$file" .cs)
    output_file="tests/StyleCop/${filename}_stylecop.txt"
    
    # Run dotnet build to trigger StyleCop checks and redirect the output to the file
    dotnet build $file --no-restore --warnaserror > "$output_file" 2>&1
    
    # Log the file and the result file
    echo "$file -> $output_file" >> "$output_file"
done
