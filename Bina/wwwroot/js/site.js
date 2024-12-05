<script>
    document.querySelectorAll("form").forEach(form => {
        form.addEventListener("submit", function (event) {
            // Example: Check for password mismatch in the reset password form
            if (form.querySelector("[name='NewPassword']") && form.querySelector("[name='ConfirmPassword']")) {
                const password = form.querySelector("[name='NewPassword']").value;
                const confirmPassword = form.querySelector("[name='ConfirmPassword']").value;
                if (password !== confirmPassword) {
                    event.preventDefault();
                    alert("Passwords do not match!");
                }
            }
        });
    });



    document.addEventListener('DOMContentLoaded', () => {
    const deleteButtons = document.querySelectorAll('.delete-btn');

    deleteButtons.forEach(button => {
        button.addEventListener('click', async () => {
            const userId = button.getAttribute('data-user-id');

            const confirmation = confirm('Are you sure you want to delete this user?');
            if (!confirmation) return;

            try {
                const response = await fetch(`/YourController/Delete/${userId}`, {
                    method: 'DELETE',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                });

                if (response.ok) {
                    alert('User deleted successfully!');
                    location.reload();
                } else {
                    alert('Failed to delete user. Please try again.');
                }
            } catch (error) {
                console.error('Error:', error);
                alert('An error occurred while deleting the user.');
            }
        });
    });
});
</script>