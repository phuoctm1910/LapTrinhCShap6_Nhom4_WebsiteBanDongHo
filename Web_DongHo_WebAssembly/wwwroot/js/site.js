function initializeScrollHandler(dotnetHelper) {
    window.addEventListener('scroll', function () {
        if (window.scrollY > 50) {
            dotnetHelper.invokeMethodAsync('SetNavbarClass', 'fixed-top');
        } else {
            dotnetHelper.invokeMethodAsync('SetNavbarClass', 'sticky-top');
        }
    });
}
