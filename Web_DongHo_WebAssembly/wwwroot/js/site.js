$(document).ready(function () {
    $(window).scroll(function () {
        if ($(this).scrollTop() > 50) {
            $('.navbar').removeClass('sticky-top').addClass('fixed-top');
        } else {
            $('.navbar').removeClass('fixed-top').addClass('sticky-top');
        }
    });
    $('.addProduct').click(function () {
        var productId = $(this).data('product-id');
        addProductToBill(productId);
    })

    function addProductToBill(productId) {
        $.ajax({
            url: '@Url.Action("CreateBill", "Bill")',
            type: 'POST',
            data: { productId: productId },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                } else {
                    console.log(response.message);
                }
            },
            error: function (respone) {
                alert('Xảy ra lỗi khi cố gắng thêm sản phẩm vào bill.');
                console.log(respone.message);
            }
        });
    }
});