/**
 * BarcodeScanner — wraps @zxing/library for live camera barcode scanning.
 * Requires the ZXing UMD bundle to be loaded before this script, and
 * the modal elements (bs-modal, bs-video, bs-status) to exist in the DOM.
 */
const BarcodeScanner = (function () {
    'use strict';

    let reader = null;

    function open(onDetect) {
        const modal = document.getElementById('bs-modal');
        modal.style.display = 'flex';
        setStatus('Starting camera\u2026');

        reader = new ZXing.BrowserMultiFormatReader();

        const video = document.getElementById('bs-video');

        reader.decodeFromVideoDevice(null, video, function (result, err) {
            if (result) {
                close();
                onDetect(result.getText());
                return;
            }
            if (err && !(err instanceof ZXing.NotFoundException)) {
                setStatus('Camera error: ' + (err.message || err));
            }
        }).then(function () {
            setStatus('Point camera at a barcode\u2026');
        }).catch(function (err) {
            setStatus('Could not access camera: ' + (err.message || err));
        });
    }

    function close() {
        if (reader) {
            try { reader.reset(); } catch (_) { }
            reader = null;
        }
        document.getElementById('bs-modal').style.display = 'none';
    }

    function setStatus(msg) {
        const el = document.getElementById('bs-status');
        if (el) el.textContent = msg;
    }

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' && reader) close();
    });

    return { open: open, close: close };
}());
