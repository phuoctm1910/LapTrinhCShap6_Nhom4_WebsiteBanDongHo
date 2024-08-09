window.hideExtraProducts = function (itemsPerPage) {
    var products = document.querySelectorAll('.product');
    products.forEach((product, index) => {
        if (index >= itemsPerPage) {
            product.style.display = 'none';
        }
    });
};

window.showMoreItems = function (itemsPerPage) {
    var products = document.querySelectorAll('.product');
    var hiddenItems = Array.from(products).filter(function (product) {
        return product.style.display === 'none';
    });

    var numToShow = Math.min(hiddenItems.length, itemsPerPage);
    for (var i = 0; i < numToShow; i++) {
        hiddenItems[i].style.display = 'block';
    }

    window.visibleCount += numToShow;

    window.toggleShowLessButton(window.visibleCount, itemsPerPage);
    window.toggleShowMoreButton(window.visibleCount, window.totalItems);
};

window.showLessItems = function (itemsPerPage) {
    var products = document.querySelectorAll('.product');
    var visibleItems = Array.from(products).filter(function (product) {
        return product.style.display === 'block';
    });

    var numToHide = Math.min(window.visibleCount - itemsPerPage, itemsPerPage);
    for (var i = 0; i < numToHide; i++) {
        visibleItems[visibleItems.length - 1 - i].style.display = 'none';
    }

    window.visibleCount -= numToHide;

    window.toggleShowLessButton(window.visibleCount, itemsPerPage);
    document.getElementById('btnShowMore').style.display = 'block';
};

window.toggleShowLessButton = function (visibleCount, itemsPerPage) {
    var btnShowLess = document.getElementById('btnShowLess');
    if (btnShowLess) { // Check if the element exists
        if (visibleCount > itemsPerPage) {
            btnShowLess.style.display = 'block';
        } else {
            btnShowLess.style.display = 'none';
        }
    }
};

window.toggleShowMoreButton = function (visibleCount, totalItems) {
    var btnShowMore = document.getElementById('btnShowMore');
    if (btnShowMore) { // Check if the element exists
        if (visibleCount >= totalItems) {
            btnShowMore.style.display = 'none';
        } else {
            btnShowMore.style.display = 'block';
        }
    }
};

window.initializeProductDisplay = function () {
    console.log("Initializing product display...");

    var observer = new MutationObserver(function (mutations, observer) {
        console.log("Checking for products in DOM...");
        if (document.querySelectorAll('.product').length > 0) {
            console.log("Products found, initializing...");

            observer.disconnect(); // Stop observing once elements are found

            window.itemsPerPage = 8;
            window.visibleCount = window.itemsPerPage;
            window.totalItems = document.querySelectorAll('.product').length;

            window.hideExtraProducts(window.itemsPerPage);
            window.toggleShowLessButton(window.visibleCount, window.itemsPerPage);
            window.toggleShowMoreButton(window.visibleCount, window.totalItems);

            var btnShowMore = document.getElementById('btnShowMore');
            if (btnShowMore) {
                console.log("Show More button found.");
                btnShowMore.addEventListener('click', function () {
                    window.showMoreItems(window.itemsPerPage);
                });
            }

            var btnShowLess = document.getElementById('btnShowLess');
            if (btnShowLess) {
                console.log("Show Less button found.");
                btnShowLess.addEventListener('click', function () {
                    window.showLessItems(window.itemsPerPage);
                });
            }
        } else {
            console.log("Products not found, still waiting...");
        }
    });

    observer.observe(document.body, {
        childList: true,
        subtree: true
    });
};

