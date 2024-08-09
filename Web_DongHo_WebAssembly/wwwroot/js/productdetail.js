function initializeMagnifierAndCarousel() {
    const observer = new MutationObserver(() => { // Hàm ??i load lên h?t nè
        const smallImg = document.querySelector(".small-img");
        const bigImg = document.querySelector(".big-img");
        const magnifier = document.querySelector(".magnifier");
        const owlCarousel = document.querySelector('.owl-carousel');

        console.log("smallImg:", smallImg);
        console.log("bigImg:", bigImg);
        console.log("magnifier:", magnifier);
        console.log("owlCarousel:", owlCarousel);

        if (smallImg && bigImg && magnifier && owlCarousel) {
            observer.disconnect(); // tìm th?y thì disconnect

            setupMagnifier(smallImg, bigImg, magnifier);
            setupCarousel(owlCarousel);
            setupRefundModal();
        }
    });

    observer.observe(document.body, { // ch?y
        childList: true,
        subtree: true
    });
}

function setupMagnifier(smallImg, bigImg, magnifier) {
    let pro_width = 0;
    let pro_height = 0;

    bigImg.style.background = `url('${smallImg.src}') no-repeat`;

    function handleMagnifierMouseMove(e) {
        if (!pro_width && !pro_height) {
            const img_obj = new Image();
            img_obj.src = smallImg.src;
            img_obj.onload = () => {
                pro_width = img_obj.width;
                pro_height = img_obj.height;
            };
        } else {
            const img_offset = magnifier.getBoundingClientRect();
            const mx = e.pageX - img_offset.left;
            const my = e.pageY - img_offset.top;

            if (mx < magnifier.clientWidth && my < magnifier.clientHeight && mx > 0 && my > 0) {
                bigImg.style.display = "block";
            } else {
                bigImg.style.display = "none";
            }
            if (bigImg.style.display === "block") {
                const rx = Math.round(mx / smallImg.width * pro_width - bigImg.clientWidth / 2) * -1;
                const ry = Math.round(my / smallImg.height * pro_height - bigImg.clientHeight / 2) * -1;
                const bgp = `${rx}px ${ry}px`;
                const px = mx - bigImg.clientWidth / 2;
                const py = my - bigImg.clientHeight / 2;
                bigImg.style.left = `${px}px`;
                bigImg.style.top = `${py}px`;
                bigImg.style.backgroundPosition = bgp;
            }
        }
    }

    magnifier.addEventListener("mousemove", handleMagnifierMouseMove);

}

function setupCarousel(owlCarousel) {
    const firstItem = owlCarousel.querySelector(".item:first-child");
    if (firstItem) {
        firstItem.classList.add("selected");
    }

    const items = owlCarousel.querySelectorAll(".item");
    items.forEach(function (item) {
        item.addEventListener("click", function () {
            const imgSrc = this.querySelector("img").src;
            const mainImage = document.getElementById("main-image");
            if (mainImage) {
                mainImage.src = imgSrc;
            }
            items.forEach(function (i) {
                i.classList.remove("selected");
            });
            this.classList.add("selected");

            const bigImg = document.querySelector(".big-img");
            if (bigImg) {
                bigImg.style.background = `url('${imgSrc}') no-repeat`;
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
    const refundButton = document.querySelector('#refund');
    if (refundButton) {
        refundButton.addEventListener('click', function () {
            const popupModal = document.getElementById('popupModal');
            if (popupModal) {
                const myModal = new bootstrap.Modal(popupModal);
                myModal.show();
            }
        });
    }
}

function cleanup(magnifier, handleMagnifierMouseMove) {
    if (magnifier) {
        magnifier.removeEventListener("mousemove", handleMagnifierMouseMove);
    }
}
