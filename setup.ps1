$downloadLinks = ((Invoke-WebRequest https://github.com/bluemagic123/tModLoader/releases).Links | Where-Object -Property outerText -like "*tModLoader.Windows.*")

$latest = $downloadLinks[0]

$latest.outerText -match "v\d+(\.\d+){0,3}"

$link = "https://github.com" + $latest.href

Write-Output ("Detected latest tModLoader version: " + $matches[0])
Write-Output ("Downloading from " + $link)

Try {
    Invoke-WebRequest $link -OutFile "tml.zip"
}
Catch {
    (New-Object System.Net.WebClient).DownloadFile($link, "tml.zip")
}

$tmlpath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath("tml.zip")
$tmppath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath("References")

Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::ExtractToDirectory($tmlpath, $tmppath)

Remove-Item $tmlpath;