/**
 * Blazor JSInterop bridge for the BarcodeScanner.
 * Calls back into a .NET DotNetObjectReference when a code is detected.
 */

window.openBarcodeScanner = function (dotNetHelper) {
    BarcodeScanner.open(function (code) {
        dotNetHelper.invokeMethodAsync('OnBarcodeScanned', code);
    });
};

window.openBarcodeScannerForSearch = function (dotNetHelper) {
    BarcodeScanner.open(function (code) {
        dotNetHelper.invokeMethodAsync('OnBarcodeSearchResult', code);
    });
};
