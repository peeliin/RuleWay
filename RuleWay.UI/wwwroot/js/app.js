const API_BASE_URL = "https://localhost:7080";

let currentPage = 1;
const pageSize = 5;
let filterActive = false;
let categories = {};
let deleteCallback = null;
let selectedImageFile = null;

const productModal = new bootstrap.Modal(document.getElementById("productModal"));
const categoryModal = new bootstrap.Modal(document.getElementById("categoryModal"));
const deleteModal = new bootstrap.Modal(document.getElementById("deleteModal"));

$(document).ready(function () {
    loadCategories().always(function () {
        loadProducts();
    });

    $(".nav-item").click(function (e) {
        e.preventDefault();
        $(".nav-item").removeClass("active");
        $(this).addClass("active");

        const page = $(this).data("page");

        if (page === "products") {
            $("#productsPage").show();
            $("#categoriesPage").hide();
            $("#pageTitle").text("Ürünler");
        } else {
            $("#productsPage").hide();
            $("#categoriesPage").show();
            $("#pageTitle").text("Kategoriler");
            loadCategoryTable();
        }

        if ($(window).width() <= 768) {
            $("#sidebar").removeClass("open");
        }
    });

    $("#sidebarToggle").click(function () {
        $("#sidebar").toggleClass("open");
    });

    $("#addProductBtn").click(function () {
        $("#productForm")[0].reset();
        $("#productId").val("");
        $("#productModalTitle").text("Ürün Ekle");
        resetImagePreview();
        productModal.show();
    });

    $("#filterBtn").click(function () {
        currentPage = 1;
        filterActive = true;
        loadProducts();
    });

    $("#clearBtn").click(function () {
        $("#keywordInput").val("");
        $("#minStockInput").val("");
        $("#maxStockInput").val("");
        currentPage = 1;
        filterActive = false;
        loadProducts();
    });

    $("#previousBtn").click(function () {
        if (currentPage > 1) {
            currentPage--;
            loadProducts();
        }
    });

    $("#nextBtn").click(function () {
        currentPage++;
        loadProducts();
    });

    $("#productForm").submit(function (e) {
        e.preventDefault();
        saveProduct();
    });

    $(document).on("click", ".edit-product-btn", function () {
        openEditProductModal($(this).data("id"));
    });

    $(document).on("click", ".delete-product-btn", function () {
        const id = $(this).data("id");
        const title = $(this).closest("tr").find(".product-name").text().trim();
        openDeleteModal(
            `"${title}" ürünü silmek istediğinize emin misiniz?`,
            function () { deleteProduct(id); }
        );
    });

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

    $("#confirmDeleteBtn").click(function () {
        if (deleteCallback) {
            deleteCallback();
        }
    });


    $("#productImage").change(function () {
        const file = this.files[0];
        if (file) {
            selectedImageFile = file;
            const reader = new FileReader();
            reader.onload = function (e) {
                $("#imagePreview").attr("src", e.target.result).show();
                $("#imagePlaceholder").hide();
            };
            reader.readAsDataURL(file);
        }
    });
});

function resetImagePreview() {
    selectedImageFile = null;
    $("#imagePreview").attr("src", "").hide();
    $("#imagePlaceholder").show();
    $("#productImage").val("");
}

function loadCategories() {
    return $.ajax({
        url: `${API_BASE_URL}/api/Category`,
        type: "GET",
        success: function (data) {
            categories = {};
            $("#productCategory").empty().append('<option value="">Kategori Yok</option>');

            data.forEach(function (cat) {
                categories[cat.id] = cat.name;
                $("#productCategory").append(
                    `<option value="${cat.id}">${esc(cat.name)}</option>`
                );
            });
        },
        error: function () {
            showToast("Kategoriler yüklenemedi.", "error");
        }
    });
}

function loadProducts() {
    let url = `${API_BASE_URL}/api/Product`;
    const data = { page: currentPage, pageSize: pageSize };

    if (filterActive) {
        url = `${API_BASE_URL}/api/Product/filter`;
        const keyword = $("#keywordInput").val().trim();
        const minStock = $("#minStockInput").val();
        const maxStock = $("#maxStockInput").val();

        if (keyword !== "") data.keyword = keyword;
        if (minStock !== "") data.minStock = minStock;
        if (maxStock !== "") data.maxStock = maxStock;
    }

    $.ajax({
        url: url,
        type: "GET",
        data: data,
        success: function (response) {
            renderProducts(response);
        },
        error: function (xhr) {
            showToast(getErrorMessage(xhr), "error");
        }
    });
}

function renderProducts(response) {
    const tbody = $("#productTableBody");
    tbody.empty();

    if (!response.items || response.items.length === 0) {
        tbody.append(`<tr><td colspan="6" class="empty-row">Ürün bulunamadı.</td></tr>`);
    } else {
        response.items.forEach(function (p) {
            const catName = p.categoryId && categories[p.categoryId]
                ? categories[p.categoryId]
                : "—";

            const status = p.isLive
                ? '<span class="badge-live">Yayında</span>'
                : '<span class="badge-offline">Yayında Değil</span>';

            const thumb = p.imageUrl
                ? `<img src="${API_BASE_URL}${p.imageUrl}" class="product-thumb" alt="">`
                : '<div class="product-thumb-placeholder"><i class="bi bi-image"></i></div>';

            tbody.append(`
                <tr>
                    <td>${thumb}</td>
                    <td>
                        <div class="product-info">
                            <div class="product-name">${esc(p.title)}</div>
                            <div class="product-desc">${esc(p.description)}</div>
                        </div>
                    </td>
                    <td>${esc(catName)}</td>
                    <td>${p.stockQuantity}</td>
                    <td>${status}</td>
                    <td>
                        <button class="action-btn edit edit-product-btn" data-id="${p.id}">
                            <i class="bi bi-pencil"></i> Güncelle
                        </button>
                        <button class="action-btn delete delete-product-btn" data-id="${p.id}">
                            <i class="bi bi-trash3"></i> Sil
                        </button>
                    </td>
                </tr>
            `);
        });
    }

    const totalPages = Math.max(1, Math.ceil(response.totalCount / pageSize));
    $("#pageInfo").text(`Sayfa ${response.page} / ${totalPages}`);
    $("#previousBtn").prop("disabled", response.page <= 1);
    $("#nextBtn").prop("disabled", response.page >= totalPages);
}

function openEditProductModal(id) {
    $.ajax({
        url: `${API_BASE_URL}/api/Product/${id}`,
        type: "GET",
        success: function (p) {
            $("#productId").val(p.id);
            $("#productTitle").val(p.title);
            $("#productDescription").val(p.description);
            $("#productStock").val(p.stockQuantity);
            $("#productCategory").val(p.categoryId ?? "");
            $("#productModalTitle").text("Ürün Güncelle");

            selectedImageFile = null;
            $("#productImage").val("");

            if (p.imageUrl) {
                $("#imagePreview").attr("src", `${API_BASE_URL}${p.imageUrl}`).show();
                $("#imagePlaceholder").hide();
            } else {
                resetImagePreview();
            }

            productModal.show();
        },
        error: function (xhr) {
            showToast(getErrorMessage(xhr), "error");
        }
    });
}

function saveProduct() {
    const id = $("#productId").val();
    const categoryValue = $("#productCategory").val();

    const product = {
        title: $("#productTitle").val().trim(),
        description: $("#productDescription").val().trim(),
        stockQuantity: Number($("#productStock").val()),
        categoryId: categoryValue === "" ? null : Number(categoryValue)
    };

    const isUpdate = id !== "";

    $.ajax({
        url: isUpdate
            ? `${API_BASE_URL}/api/Product/${id}`
            : `${API_BASE_URL}/api/Product`,
        type: isUpdate ? "PUT" : "POST",
        contentType: "application/json",
        data: JSON.stringify(product),
        success: function (data, textStatus, xhr) {
            const productId = isUpdate ? id : data.id;

            if (selectedImageFile && productId) {
                uploadImage(productId, function () {
                    productModal.hide();
                    showToast(isUpdate ? "Ürün başarıyla güncellendi." : "Ürün başarıyla eklendi.", "success");
                    loadProducts();
                });
            } else {
                productModal.hide();
                showToast(isUpdate ? "Ürün başarıyla güncellendi." : "Ürün başarıyla eklendi.", "success");
                loadProducts();
            }
        },
        error: function (xhr) {
            showToast(getErrorMessage(xhr), "error");
        }
    });
}

function uploadImage(productId, callback) {
    const formData = new FormData();
    formData.append("file", selectedImageFile);

    $.ajax({
        url: `${API_BASE_URL}/api/Product/${productId}/image`,
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function () {
            selectedImageFile = null;
            if (callback) callback();
        },
        error: function (xhr) {
            showToast(getErrorMessage(xhr), "error");
            if (callback) callback();
        }
    });
}

function deleteProduct(id) {
    $.ajax({
        url: `${API_BASE_URL}/api/Product/${id}`,
        type: "DELETE",
        success: function () {
            deleteModal.hide();
            showToast("Ürün başarıyla silindi.", "success");
            loadProducts();
        },
        error: function (xhr) {
            deleteModal.hide();
            showToast(getErrorMessage(xhr), "error");
        }
    });
}

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
            loadCategories();
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
            loadCategories();
        },
        error: function (xhr) {
            deleteModal.hide();
            showToast(getErrorMessage(xhr), "error");
        }
    });
}

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
