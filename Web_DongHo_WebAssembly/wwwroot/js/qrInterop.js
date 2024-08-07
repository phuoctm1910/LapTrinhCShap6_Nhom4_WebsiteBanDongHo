function generateQRCode(text, elementId) {
    var element = document.getElementById(elementId);

    element.innerHTML = "";

    new QRCode(element, {
        text: text,
        width: 80,
        height: 80,
        colorDark: "#000000",
        colorLight: "#ffffff",
        correctLevel: QRCode.CorrectLevel.H
    });
}

