const API_BASE_URL = "https://localhost:7080";
let deleteCallback = null;
let deleteModal;

$(document).ready(function () {
    deleteModal = new bootstrap.Modal(document.getElementById("deleteModal"));
    
    $("#confirmDeleteBtn").click(function () {
        if (deleteCallback) {
            deleteCallback();
        }
    });
});

function openDeleteModal(message, callback) {
    deleteCallback = callback;
    $("#deleteMessage").text(message);
    deleteModal.show();
}

function showToast(message, type) {
    const icon = type === "success" ? "bi-check-circle-fill" : "bi-x-circle-fill";
    const toastClass = type === "success" ? "toast-success" : "toast-error";

    const toastId = "toast-" + Date.now();
    const html = `
        <div id="${toastId}" class="toast custom-toast ${toastClass}" role="alert">
            <div class="toast-body">
                <i class="bi ${icon}"></i>
                <span>${esc(message)}</span>
            </div>
        </div>
    `;

    $("#toastContainer").append(html);
    const toastEl = document.getElementById(toastId);
    const toast = new bootstrap.Toast(toastEl, { delay: 3500 });
    toast.show();

    $(toastEl).on("hidden.bs.toast", function () {
        $(this).remove();
    });
}

function getErrorMessage(xhr) {
    if (xhr.responseJSON && xhr.responseJSON.message) {
        return xhr.responseJSON.message;
    }
    return "İşlem sırasında bir hata oluştu.";
}

function esc(value) {
    return $("<div>").text(value ?? "").html();
}
