param(
  [string[]]$folders=$(throw "-folders required")
)

$dic = @{}
foreach ($folder in $folders) {
  Get-ChildItem $folder -Recurse | where { $_.Attributes -ne 'Directory' } | foreach {
    $dic[$_.Name.ToLower()] += 1
  }
}

$dic.GetEnumerator() | sort Key | foreach {
  if ($_.Value -gt 1) {
    Write-Host "$($_.Key): $($_.Value)"
  }
}