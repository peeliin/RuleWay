let currentPage = 1;
const pageSize = 5;
let filterActive = false;
let categories = {};
let productToDeleteId = null;

const productModal = new bootstrap.Modal(
    document.getElementById("productModal")
);

const deleteModal = new bootstrap.Modal(
    document.getElementById("deleteModal")
);

$(document).ready(function () {

    loadCategories().always(function () {
        loadProducts();
    });

    $("#addProductBtn").click(function () {
        $("#productForm")[0].reset();
        $("#productId").val("");
        $("#modalTitle").text("Add Product");

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

    $("#productForm").submit(function (event) {
        event.preventDefault();

        saveProduct();
    });

    $(document).on("click", ".edit-btn", function () {
        const id = $(this).data("id");

        openEditModal(id);
    });

    $(document).on("click", ".delete-btn", function () {
        const id = $(this).data("id");

        const title = $(this)
            .closest("tr")
            .find(".product-name")
            .text()
            .trim();

        productToDeleteId = id;

        $("#deleteMessage").text(
            `Are you sure you want to delete "${title}"?`
        );

        deleteModal.show();
    });

    $("#confirmDeleteBtn").click(function () {
        if (productToDeleteId !== null) {
            deleteProduct(productToDeleteId);
        }
    });

});

function loadCategories() {

    return $.ajax({
        url: "/api/Category",
        type: "GET",

        success: function (data) {

            categories = {};

            $("#productCategory")
                .empty()
                .append(
                    '<option value="">No Category</option>'
                );

            data.forEach(function (category) {

                categories[category.id] = category.name;

                $("#productCategory").append(
                    `<option value="${category.id}">
                        ${escapeHtml(category.name)}
                    </option>`
                );

            });

        },

        error: function () {
            showMessage(
                "Categories could not be loaded.",
                "danger"
            );
        }
    });

}

function loadProducts() {

    let url = "/api/Product";

    const data = {
        page: currentPage,
        pageSize: pageSize
    };

    if (filterActive) {

        url = "/api/Product/filter";

        const keyword = $("#keywordInput").val().trim();
        const minStock = $("#minStockInput").val();
        const maxStock = $("#maxStockInput").val();

        if (keyword !== "") {
            data.keyword = keyword;
        }

        if (minStock !== "") {
            data.minStock = minStock;
        }

        if (maxStock !== "") {
            data.maxStock = maxStock;
        }

    }

    $.ajax({
        url: url,
        type: "GET",
        data: data,

        success: function (response) {
            renderProducts(response);
        },

        error: function (xhr) {
            showError(xhr);
        }
    });

}

function renderProducts(response) {

    const tbody = $("#productTableBody");

    tbody.empty();

    if (!response.items || response.items.length === 0) {

        tbody.append(`
            <tr>
                <td colspan="5" class="empty-row">
                    No products found.
                </td>
            </tr>
        `);

    } else {

        response.items.forEach(function (product) {

            const categoryName =
                product.categoryId &&
                    categories[product.categoryId]
                    ? categories[product.categoryId]
                    : "No Category";

            const status =
                product.isLive
                    ? '<span class="status-live">Live</span>'
                    : '<span class="status-offline">Not Live</span>';

            tbody.append(`
                <tr>

                    <td>
                        <div class="product-name">
                            ${escapeHtml(product.title)}
                        </div>

                        <div class="product-description">
                            ${escapeHtml(product.description)}
                        </div>
                    </td>

                    <td>
                        ${escapeHtml(categoryName)}
                    </td>

                    <td>
                        ${product.stockQuantity}
                    </td>

                    <td>
                        ${status}
                    </td>

                    <td>
                        <button class="edit-btn"
                                data-id="${product.id}">
                            Edit
                        </button>

                        <button class="delete-btn"
                                data-id="${product.id}">
                            Delete
                        </button>
                    </td>

                </tr>
            `);

        });

    }

    const totalPages = Math.max(
        1,
        Math.ceil(response.totalCount / pageSize)
    );

    $("#pageInfo").text(
        `Page ${response.page} of ${totalPages}`
    );

    $("#previousBtn").prop(
        "disabled",
        response.page <= 1
    );

    $("#nextBtn").prop(
        "disabled",
        response.page >= totalPages
    );

}

function openEditModal(id) {

    $.ajax({
        url: `/api/Product/${id}`,
        type: "GET",

        success: function (product) {

            $("#productId").val(product.id);

            $("#productTitle").val(
                product.title
            );

            $("#productDescription").val(
                product.description
            );

            $("#productStock").val(
                product.stockQuantity
            );

            $("#productCategory").val(
                product.categoryId ?? ""
            );

            $("#modalTitle").text(
                "Edit Product"
            );

            productModal.show();

        },

        error: function (xhr) {
            showError(xhr);
        }
    });

}

function saveProduct() {

    const id = $("#productId").val();

    const categoryValue =
        $("#productCategory").val();

    const product = {

        title:
            $("#productTitle")
                .val()
                .trim(),

        description:
            $("#productDescription")
                .val()
                .trim(),

        stockQuantity:
            Number(
                $("#productStock").val()
            ),

        categoryId:
            categoryValue === ""
                ? null
                : Number(categoryValue)

    };

    const isUpdate = id !== "";

    $.ajax({

        url: isUpdate
            ? `/api/Product/${id}`
            : "/api/Product",

        type: isUpdate
            ? "PUT"
            : "POST",

        contentType: "application/json",

        data: JSON.stringify(product),

        success: function () {

            productModal.hide();

            showMessage(
                isUpdate
                    ? "Product updated successfully."
                    : "Product added successfully.",
                "success"
            );

            loadProducts();

        },

        error: function (xhr) {
            showError(xhr);
        }

    });

}

function deleteProduct(id) {

    $.ajax({
        url: `/api/Product/${id}`,
        type: "DELETE",

        success: function () {

            deleteModal.hide();

            productToDeleteId = null;

            showMessage(
                "Product deleted successfully.",
                "success"
            );

            loadProducts();

        },

        error: function (xhr) {

            deleteModal.hide();

            productToDeleteId = null;

            showError(xhr);

        }
    });

}

function showMessage(message, type) {

    $("#alertContainer").html(`
        <div class="alert alert-${type}
                    alert-dismissible fade show"
             role="alert">

            ${escapeHtml(message)}

            <button type="button"
                    class="btn-close"
                    data-bs-dismiss="alert">
            </button>

        </div>
    `);

}

function showError(xhr) {

    let message = "Something went wrong.";

    if (
        xhr.responseJSON &&
        xhr.responseJSON.message
    ) {
        message =
            xhr.responseJSON.message;
    }

    showMessage(
        message,
        "danger"
    );

}

function escapeHtml(value) {

    return $("<div>")
        .text(value ?? "")
        .html();

}