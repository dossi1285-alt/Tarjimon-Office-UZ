$ErrorActionPreference='Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$source = Join-Path $root 'TarjimonOfficeUZ.Setup.Preflight\ProgramV110.cs'
if (-not (Test-Path $source)) { throw "ProgramV110.cs topilmadi: $source" }
$backup = Join-Path $root 'ProgramV110.cs.1.1.0.backup'
if (-not (Test-Path $backup)) { Copy-Item $source $backup -Force }
$text = Get-Content $source -Raw -Encoding UTF8

$oldFilter = '.Where(x => x.IsOwnProduct || x.Score >= 35)'
if (-not $text.Contains($oldFilter)) { throw 'Stabil candidate filter qatori topilmadi.' }
$text = $text.Replace($oldFilter, '.Where(IsRelevantCandidate)')

$marker = '        private static void ScanOfficeAddins(List<AddinCandidate> list, RegistryView[] views)'
if (-not $text.Contains($marker)) { throw 'ScanOfficeAddins marker topilmadi.' }

$methodPattern = '(?s)        private static bool IsRelevantCandidate\(AddinCandidate x\)\s*\{.*?\n        \}\s*\n\s*        private static void ScanOfficeAddins'
$method = @'
        private static bool IsRelevantCandidate(AddinCandidate x)
        {
            if (x == null) return false;
            if (x.IsOwnProduct) return true;

            var productPublisher = NormalizeSearchText(x.Product, x.Publisher);
            var officeEvidence = NormalizeSearchText(x.Product, x.Publisher, x.Evidence, x.InstallLocation);
            if (IsKnownNonTranslatorProduct(productPublisher)) return false;

            var productFunctional = FunctionWords.Any(w => productPublisher.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
            var productStrongPair = StrongFunctionPairs.Any(w => productPublisher.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);
            var officeHost = string.Equals(x.Host, "Word", StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(x.Host, "Excel", StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(x.Host, "Office", StringComparison.OrdinalIgnoreCase);

            // Office add-in/startup: product/publisher signal is primary. Evidence may support it.
            if (officeHost)
                return productStrongPair || productFunctional ||
                       StrongFunctionPairs.Any(w => officeEvidence.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0);

            // Windows Uninstall entries: do NOT use arbitrary DLL/file metadata as the main
            // translator signal; it causes false positives such as browsers and utilities.
            // Keep explicit functional names and verified functional brand names only.
            if (IsVerifiedFunctionalBrand(productPublisher)) return true;
            return productStrongPair || productFunctional;
        }

        private static bool IsKnownNonTranslatorProduct(string productText)
        {
            if (string.IsNullOrWhiteSpace(productText)) return false;
            var known = new[] {
                "igor pavlov", "7 zip", "7zip", "winrar", "google chrome", "chrome",
                "mozilla firefox", "firefox", "lightshot", "startallback", "startallback++", "startisback",
                "telegram desktop", "telegram", "github desktop", "git", "visual studio code",
                "vscode", "visual studio", "microsoft silverlight", "silverlight",
                "e imzo", "uzcrypto", "zoom workplace", "zoom", "easeus data recovery",
                "minitool power data recovery", "workflow manager",
                "microsoft access", "microsoft excel", "microsoft word", "microsoft outlook",
                "microsoft powerpoint", "microsoft publisher", "microsoft project", "microsoft office",
                "microsoft proofing", "microsoft language pack", "microsoft visual studio tools",
                "microsoft .net", "microsoft net framework", "microsoft edge", "microsoft teams"
            };
            return known.Any(p => productText.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static bool IsVerifiedFunctionalBrand(string productText)
        {
            if (string.IsNullOrWhiteSpace(productText)) return false;
            var brands = new[] {
                "savodxon"
            };
            return brands.Any(p => productText.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private static void ScanOfficeAddins
'@
$newText = [regex]::Replace($text, $methodPattern, $method, 1)
if ($newText -eq $text) { throw 'IsRelevantCandidate qismi topilmadi.' }
$text = $newText

$identityPattern = '(?s)        private static string BuildProductIdentity\(AddinCandidate item\)\s*\{.*?\n        \}\s*\n\s*        private static AddinCandidate MergeCandidate'
$identityReplacement = @'
        private static string BuildProductIdentity(AddinCandidate item)
        {
            if (item.IsOwnProduct) return "OWN:tarjimon-office-uz";

            var product = NormalizeIdentity(item.Product);
            var publisher = NormalizeIdentity(item.Publisher);

            // Functional canonical identity comes BEFORE MSI identity. This is important:
            // one source may be a Word Add-in with no uninstall command while another source
            // may be a Windows Uninstall entry with an MSI product code. If both describe the
            // same functional product (e.g. TransLit/Translit), they must merge first.
            if (IsMergeableFunctionalProduct(product))
                return "FUNC:" + product;

            var code = ExtractProductCode(item.UninstallString);
            if (!string.IsNullOrWhiteSpace(code)) return "MSI:" + code;

            return "APP:" + product + "|" + publisher;
        }

        private static bool IsMergeableFunctionalProduct(string product)
        {
            if (string.IsNullOrWhiteSpace(product)) return false;
            var names = new[] {
                "translit", "translit gt", "translitgt", "translator", "transliterator", "translation",
                "savodxon"
            };
            return names.Contains(product, StringComparer.OrdinalIgnoreCase);
        }

        private static AddinCandidate MergeCandidate
'@
$newText = [regex]::Replace($text, $identityPattern, $identityReplacement, 1)
if ($newText -eq $text) { throw 'BuildProductIdentity qismi topilmadi.' }

Set-Content -Path $source -Value $newText -Encoding UTF8
Write-Host '1.1.7 strict filter + duplicate merge patch qo`llandi.'
