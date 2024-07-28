window.initializeProductDisplay = function () {
    var itemsPerPage = 8; // Số lượng sản phẩm hiển thị mỗi lần
    var totalItems = $('.product').length; // Tổng số sản phẩm
    var visibleCount = itemsPerPage; // Bắt đầu hiển thị số lượng ban đầu

    // Ẩn các sản phẩm vượt quá số lượng ban đầu
    $('.product').slice(visibleCount).hide();

    // Kiểm tra số lượng sản phẩm ban đầu để ẩn nút "Ẩn Bớt"
    if (totalItems <= itemsPerPage) {
        $('#btnShowLess').hide(); // Ẩn nút "Ẩn Bớt" khi số lượng sản phẩm ít hơn hoặc bằng itemsPerPage
    }

    // Hàm hiển thị thêm sản phẩm
    function showMore() {
        var hiddenItems = $('.product:hidden');
        var numToShow = Math.min(hiddenItems.length, itemsPerPage);
        hiddenItems.slice(0, numToShow).slideDown();
        visibleCount += numToShow;

        // Hiển thị nút "Ẩn Bớt" khi đã hiển thị hết số lượng sản phẩm ban đầu
        if (visibleCount > itemsPerPage) {
            $('#btnShowLess').show(); // Hiển thị nút "Ẩn Bớt"
        }

        // Ẩn nút "Xem Thêm" nếu đã hiển thị hết sản phẩm
        if (visibleCount >= totalItems) {
            $('#btnShowMore').hide(); // Ẩn nút "Xem Thêm"
        }
    }

    // Hàm ẩn bớt sản phẩm
    function showLess() {
        // Tính số lượng sản phẩm cần ẩn
        var numToHide = Math.min(visibleCount - itemsPerPage, itemsPerPage);

        if (visibleCount > itemsPerPage) {
            $('.product:visible').slice(-numToHide).slideUp();
            visibleCount -= numToHide;
        }

        // Ẩn nút "Ẩn Bớt" khi không còn sản phẩm để ẩn
        if (visibleCount <= itemsPerPage) {
            $('#btnShowLess').hide(); // Ẩn nút "Ẩn Bớt"
        }

        // Hiển thị lại nút "Xem Thêm"
        $('#btnShowMore').show(); // Hiển thị nút "Xem Thêm"
    }

    // Sự kiện click vào nút "Xem Thêm"
    $('#btnShowMore').click(function () {
        showMore();
    });

    // Sự kiện click vào nút "Ẩn Bớt"
    $('#btnShowLess').click(function () {
        showLess();
    });
};
