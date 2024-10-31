const toast = document.querySelector('#liveToast');
const liveToast = new bootstrap.Toast(toast);
const message = document.getElementById('alertMessage');

function showToast(mess, type) {
    liveToast.hide();    
    switch (type) {
        case 'success':
            message.textContent = mess;
            toast.classList.add('bg-success');
            toast.classList.remove('bg-danger');

            break;
        case 'error':
            message.textContent = mess;
            toast.classList.add('bg-danger');
            toast.classList.remove('bg-success');
            break;
        default:
            message.textContent = mess;
            toast.classList.add('text-dark');
            toast.classList.remove('bg-success');
            toast.classList.remove('bg-danger');
            break;
    }
    liveToast.show();
}