$(function () {

    var e = $('#AuthenticatorSecretQRCode').css({ 'margin-top': '40px', 'margin-bottom': '40px' });

    var title = $('#AuthenticatorSecretQRCode').attr('title');

    if (e.length > 0) {

        new QRCode(e.get(0), title);
    }
    
});