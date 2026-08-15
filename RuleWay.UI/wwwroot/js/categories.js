let categoryModal;

$(document).ready(function () {
    categoryModal = new bootstrap.Modal(document.getElementById("categoryModal"));
    loadCategoryTable();

    $("#addCategoryBtn").click(function () {
        $("#categoryForm")[0].reset();
        $("#categoryId").val("");
        $("#categoryModalTitle").text("Kategori Ekle");
        categoryModal.show();
    });

    $("#categoryForm").submit(function (e) {
        e.preventDefault();
        saveCategory();
    });

    $(document).on("click", ".edit-category-btn", function () {
        openEditCategoryModal($(this).data("id"));
    });

    $(document).on("click", ".delete-category-btn", function () {
        const id = $(this).data("id");
        const name = $(this).closest("tr").find(".category-name-cell").text().trim();
        openDeleteModal(
            `"${name}" kategorisini silmek istediğinize emin misiniz?`,
            function () { deleteCategory(id); }
        );
    });
});

function loadCategoryTable() {
    $.ajax({
        url: `${API_BASE_URL}/api/Category`,
        type: "GET",
        success: function (data) {
            renderCategories(data);
        },
        error: function (xhr) {
            showToast(getErrorMessage(xhr), "error");
        }
    });
}

function renderCategories(data) {
    const tbody = $("#categoryTableBody");
    tbody.empty();

    if (!data || data.length === 0) {
        tbody.append(`<tr><td colspan="3" class="empty-row">Kategori bulunamadı.</td></tr>`);
    } else {
        data.forEach(function (c) {
            tbody.append(`
                <tr>
                    <td class="category-name-cell">${esc(c.name)}</td>
                    <td>${c.minimumStockQuantity}</td>
                    <td>
                        <button class="action-btn edit edit-category-btn" data-id="${c.id}">
                            <i class="bi bi-pencil"></i> Güncelle
                        </button>
                        <button class="action-btn delete delete-category-btn" data-id="${c.id}">
                            <i class="bi bi-trash3"></i> Sil
                        </button>
                    </td>
                </tr>
            `);
        });
    }
}

function openEditCategoryModal(id) {
    $.ajax({
        url: `${API_BASE_URL}/api/Category/${id}`,
        type: "GET",
        success: function (c) {
            $("#categoryId").val(c.id);
            $("#categoryName").val(c.name);
            $("#categoryMinStock").val(c.minimumStockQuantity);
            $("#categoryModalTitle").text("Kategori Güncelle");
            categoryModal.show();
        },
        error: function (xhr) {
            showToast(getErrorMessage(xhr), "error");
        }
    });
}

function saveCategory() {
    const id = $("#categoryId").val();
    const category = {
        name: $("#categoryName").val().trim(),
        minimumStockQuantity: Number($("#categoryMinStock").val())
    };

    const isUpdate = id !== "";

    $.ajax({
        url: isUpdate
            ? `${API_BASE_URL}/api/Category/${id}`
            : `${API_BASE_URL}/api/Category`,
        type: isUpdate ? "PUT" : "POST",
        contentType: "application/json",
        data: JSON.stringify(category),
        success: function () {
            categoryModal.hide();
            showToast(isUpdate ? "Kategori başarıyla güncellendi." : "Kategori başarıyla eklendi.", "success");
            loadCategoryTable();
        },
        error: function (xhr) {
            showToast(getErrorMessage(xhr), "error");
        }
    });
}

function deleteCategory(id) {
    $.ajax({
        url: `${API_BASE_URL}/api/Category/${id}`,
        type: "DELETE",
        success: function () {
            deleteModal.hide();
            showToast("Kategori başarıyla silindi.", "success");
            loadCategoryTable();
        },
        error: function (xhr) {
            deleteModal.hide();
            showToast(getErrorMessage(xhr), "error");
        }
    });
}
