/// <reference path="jquery-2.1.0.js" />
var mpps = mpps ||  {};
mpps.checkout = function (profileId, transactionType, referenceId, mppsUrl,  contactInfo, cart) {
    $("#payment_page").empty();
    //$("#payment_page").append("<iframe src='http://docs.angularjs.org/guide/' />");

   // if (!$("#payment_page").length) {
    $("#payment_page").append('<iframe id="payment_frame" name="payment_frame" frameborder="0" style="height: 980px; width: 900px" />');
   // }

    var frame = $("#payment_frame")[0].contentWindow;
    frame.document.open();
    frame.document.write('<html><head></head><body>');
    frame.document.write('<form id="payment" method="post" action="'+mppsUrl+'">');
    frame.document.write('<input type="hidden" name="PaymentDetail.BillingAddress.FirstName" value="' + contactInfo.FirstName+ '" size="25" />');
    frame.document.write('<input type="hidden" name="PaymentDetail.BillingAddress.LastName" value="' + contactInfo.LastName + '" size="25" />');
    frame.document.write('<input type="hidden" name="PaymentDetail.BillingAddress.City" value="' + contactInfo.City + '" size="25" />');
    frame.document.write('<input type="hidden" name="PaymentDetail.BillingAddress.Country" value="' + contactInfo.Country + '" size="25" />');
    frame.document.write('<input type="hidden" name="PaymentDetail.BillingAddress.StreetLine1" value="' + contactInfo.StreetLine1 + '" size="25" />');
    frame.document.write('<input type="hidden" name="PaymentDetail.BillingAddress.StreetLine2" value="' + contactInfo.StreetLine2 + '" size="25" />');
    frame.document.write('<input type="hidden" name="PaymentDetail.BillingAddress.State" value="' + contactInfo.State + '" size="25" />');
    frame.document.write('<input type="hidden" name="PaymentDetail.BillingAddress.PostalCode" value="' + contactInfo.PostalCode + '" size="25" />');
    frame.document.write('<input type="hidden" name="PaymentDetail.BillingAddress.EmailAddress" value="' + contactInfo.EmailAddress + '" size="25" />');
    frame.document.write('<input type="hidden" name="TranactionType" value="0">');
    frame.document.write('<input type="hidden" id="ReferenceNumber" name="PaymentDetail.ReferenceNumber" value="0"> ');
    frame.document.write('<input type="hidden" id="ResponseUrl" name="ResponseUrl"  value="' + '' + '">');
    frame.document.write('<input type="hidden" id="CancelUrl" name="CancelUrl"  value="' + '' + '">');
    frame.document.write('<input type="hidden" name="PaymentDetail.TaxAmount" value="' + cart.tax + '" id="TaxAmount" size="25" />');
    frame.document.write('<input type="hidden" name="PaymentDetail.TotalAmount" id="TotalAmount" value="' + cart.amount + '"  size="25" />');
    frame.document.write('<input type="hidden" name="PaymentDetail.Currency" value="USD" />');
    frame.document.write('<input type="hidden" name="ProfileID" value="' + profileId + '" />');
    frame.document.write('</form>');
    frame.document.write('</body></html>');
    frame.document.body.onload = function () {
        frame.document.getElementById("payment").submit();
    };
    frame.document.close();
    
};
