HSBsValidation.init('.js-validate-register', {
    onSubmit: data => {
        data.event.preventDefault();
        data.event.stopPropagation();

        const formData = new FormData(data.form);
        const action = data.form.getAttribute('action');
        const method = data.form.getAttribute('method');

        fetch(action, {
            method: method,
            body: formData
        })
            .then(response => response.json())
            .then(data1 => {
                if (data1.success) {
                    toast.classList.remove('bg-danger');
                    toast.classList.add('bg-success');
                }else {
                    toast.classList.remove('bg-success');
                    toast.classList.add('bg-danger');
                }
                message.textContent = data1.message;
                liveToast.show();
                document.getElementById('signupModalFormSignup').style.display = 'none';
                document.getElementById('signupModalFormLogin').style.display = 'block';
                document.getElementById('signupModalFormLogin').style.opacity = 1;
            })
            .catch(error => {
                message.textContent = error;
                toast.classList.remove('bg-success');
                toast.classList.add('bg-danger');
                liveToast.show();
            });
    }
});