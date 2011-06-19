param(
  $folder=$(throw "-folder required")
)

$dic = @{}
Get-ChildItem $folder -Recurse | where { !$_.PsIsContainer } | foreach {
  $dic[$_.Name.ToLower()] += 1
}

$dic.GetEnumerator() | sort Key | foreach {
  if ($_.Value -gt 1) {
    Write-Host "$($_.Key): $($_.Value)"
  }
}