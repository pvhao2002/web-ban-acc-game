(function() {
    let firstLoad = true;
    HSCore.components.HSNoUISlider.init('.js-nouislider');
    const priceSlider = document.getElementById('range-price');
    let debounceTimeout;
    priceSlider.noUiSlider.on('update', function(values, handle) {
        clearTimeout(debounceTimeout);

        // Thiết lập timeout mới, đợi 500ms sau khi người dùng dừng kéo
        debounceTimeout = setTimeout(function() {
            const urlParams = new URLSearchParams(window.location.search);
            const categoryId = urlParams.get('cate');
            const page = urlParams.get('page');
            // Lấy giá trị min và max
            const minPrice = values[0]; // giá trị min
            const maxPrice = values[1]; // giá trị max
            
            // xoa thap phan
            const min = Math.floor(minPrice);
            const max = Math.floor(maxPrice);
            
            // Gọi API sau khi người dùng đã dừng kéo trong 500ms
            if(firstLoad) {
                firstLoad = false;
                return;
            }
            window.location.href = `/Product?cate=${categoryId}&page=${page}&min=${min}&max=${max}`;
        }, 500); // 500ms delay sau khi người dùng ngừng kéo
    });
})();

const inputMin = document.getElementById('input-min');
const inputMax = document.getElementById('input-max');


function goToPage(page) {
    window.location.href = page;
}

function filter() {
    const min = Math.abs(inputMin.value);
    const max = Math.abs(inputMax.value);
    
    const urlParams = new URLSearchParams(window.location.search);
    const categoryId = urlParams.get('cate');
    
    window.location.href = `/Product?cate=${categoryId}&min=${min}&max=${max}`;
}

function sort(element) {
    console.log(element.value);
    const urlParams = new URLSearchParams(window.location.search);
    const categoryId = urlParams.get('cate');
    const min = urlParams.get('min');
    const max = urlParams.get('max');
    window.location.href = `/Product?cate=${categoryId}&min=${min}&max=${max}&sort=${element.value}`;
}