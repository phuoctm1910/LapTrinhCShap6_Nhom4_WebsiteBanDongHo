function initializeMagnifierAndCarousel() {
    const interval = setInterval(() => {
        const smallImg = document.querySelector(".small-img");
        const bigImg = document.querySelector(".big-img");
        const magnifier = document.querySelector(".magnifier");
        const owlCarousel = document.querySelector('.owl-carousel');
        console.log(smallImg);
        console.log(bigImg);
        console.log(magnifier);
        console.log(owlCarousel);

        if (smallImg && bigImg && magnifier && owlCarousel) {
            clearInterval(interval);
            setupMagnifier(smallImg, bigImg, magnifier);
            setupCarousel(owlCarousel);
            setupRefundModal();
        }
    }, 100); // Check every 100ms
}

function setupMagnifier(smallImg, bigImg, magnifier) {
    var pro_width = 0;
    var pro_height = 0;

    bigImg.style.background = "url('" + smallImg.src + "') no-repeat";

    magnifier.addEventListener("mousemove", function (e) {
        if (!pro_width && !pro_height) {
            var img_obj = new Image();
            img_obj.src = smallImg.src;
            pro_width = img_obj.width;
            pro_height = img_obj.height;
        } else {
            var img_offset = magnifier.getBoundingClientRect();
            var mx = e.pageX - img_offset.left;
            var my = e.pageY - img_offset.top;

            if (mx < magnifier.clientWidth && my < magnifier.clientHeight && mx > 0 && my > 0) {
                bigImg.style.display = "block";
            } else {
                bigImg.style.display = "none";
            }
            if (bigImg.style.display === "block") {
                var rx = Math.round(mx / smallImg.width * pro_width - bigImg.clientWidth / 2) * -1;
                var ry = Math.round(my / smallImg.height * pro_height - bigImg.clientHeight / 2) * -1;
                var bgp = rx + "px " + ry + "px";
                var px = mx - bigImg.clientWidth / 2;
                var py = my - bigImg.clientHeight / 2;
                bigImg.style.left = px + "px";
                bigImg.style.top = py + "px";
                bigImg.style.backgroundPosition = bgp;
            }
        }
    });
    cleanup()
}

function setupCarousel(owlCarousel) {
    var firstItem = owlCarousel.querySelector(".item:first-child");
    if (firstItem) {
        firstItem.classList.add("selected");
    }

    var items = owlCarousel.querySelectorAll(".item");
    items.forEach(function (item) {
        item.addEventListener("click", function () {
            var imgSrc = this.querySelector("img").src;
            var mainImage = document.getElementById("main-image");
            if (mainImage) {
                mainImage.src = imgSrc;
            }
            items.forEach(function (i) {
                i.classList.remove("selected");
            });
            this.classList.add("selected");

            var bigImg = document.querySelector(".big-img");
            if (bigImg) {
                bigImg.style.background = "url('" + imgSrc + "') no-repeat";
            }
        });
    });

    initializeOwlCarousel();
}

function initializeOwlCarousel() {
    const owlCarousel = $('.owl-carousel');
    if (owlCarousel.length > 0) {
        owlCarousel.owlCarousel({
            loop: false,
            margin: 10,
            nav: true,
            navText: [
                "<i class='fa fa-caret-left'></i>",
                "<i class='fa fa-caret-right'></i>"
            ],
            autoplay: false,
            autoplayHoverPause: false,
            responsive: {
                0: {
                    items: 1
                },
                600: {
                    items: 3
                },
                1000: {
                    items: 5
                }
            }
        });
    }
}

function setupRefundModal() {
    var refundButton = document.querySelector('#refund');
    if (refundButton) {
        refundButton.addEventListener('click', function () {
            var popupModal = document.getElementById('popupModal');
            if (popupModal) {
                var myModal = new bootstrap.Modal(popupModal);
                myModal.show();
            }
        });
    }
}
function cleanup() {
    var magnifier = document.querySelector(".magnifier");
    if (magnifier) {
        magnifier.removeEventListener("mousemove", handleMagnifierMouseMove);
    }
    // Additional cleanup logic if needed
}
