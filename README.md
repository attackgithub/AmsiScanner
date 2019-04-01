# AmsiScanner
A really lame C# implementation of AbstractSyntaxTree to run PowerShell content through AMSI.  Not really fit for use.

## Usage
```text
> AmsiScanner.exe
  -i, --infile=VALUE         Input file
  -h, -?, --help             Show this help
```

```text
> AmsiScanner.exe -i test.ps1

 === AST Signatures ===
  Invoke-Expression 'AMSI Test Sample: 7e72c3ce-861b-4339-8740-0ac1484c1386' == AMSI_RESULT_DETECTED
  'AMSI Test Sample: 7e72c3ce-861b-4339-8740-0ac1484c1386' == AMSI_RESULT_DETECTED

 === PSToken Signatures ===
  AMSI Test Sample: 7e72c3ce-861b-4339-8740-0ac1484c1386 == AMSI_RESULT_DETECTED
```

## Credits
[Ryan Cobb](https://twitter.com/cobbr_io) & [PSAmsi](https://github.com/cobbr/PSAmsi)