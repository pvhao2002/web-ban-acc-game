const toast = document.querySelector('#liveToast');
const liveToast = new bootstrap.Toast(toast);
const message = document.getElementById('alertMessage');
function showAlert(msg, type) {
    message.textContent = msg;
    if (type === 'success') {
        toast.classList.remove('bg-danger');
        toast.classList.remove('bg-warning');

        toast.classList.add('bg-success');
    } else if (type === 'warning') {
        toast.classList.remove('bg-danger');
        toast.classList.remove('bg-success');

        toast.classList.add('bg-warning');
    } else {
        toast.classList.remove('bg-success');
        toast.classList.remove('bg-warning');

        toast.classList.add('bg-danger');
    }
    liveToast.show();
}