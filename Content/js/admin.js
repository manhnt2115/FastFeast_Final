/* ============================================
   Content/js/admin.js
   JavaScript cho Admin Panel
   ============================================ */

(function ($) {
    'use strict';

    /* ============================================
       DOCUMENT READY
       ============================================ */
    $(document).ready(function () {
        initializeAdmin();
        initializeTableFeatures();
        initializeFormValidation();
        initializeConfirmDialogs();
        initializeTooltips();
        initializeAnimations();
        initializeImagePreviews();
        initializeSearch();
        initializeFilters();
    });

    /* ============================================
       INITIALIZE ADMIN
       ============================================ */
    function initializeAdmin() {
        console.log('FastFeast Admin Panel Loaded');

        // Show loading overlay on page transitions
        $(document).on('click', 'a[href], form', function () {
            if (!$(this).hasClass('no-loading')) {
                showLoading();
            }
        });

        // Hide loading on page load complete
        $(window).on('load', function () {
            hideLoading();
        });

        // Add fade-in animation to cards
        $('.admin-card, .stat-card').addClass('fade-in');
    }

    /* ============================================
       TABLE FEATURES
       ============================================ */
    function initializeTableFeatures() {
        // Highlight selected row
        $('.admin-table tbody tr').on('click', function () {
            $(this).toggleClass('table-active');
        });

        // Row hover effect enhancement
        $('.admin-table tbody tr').hover(
            function () {
                $(this).css('cursor', 'pointer');
            },
            function () {
                $(this).css('cursor', 'default');
            }
        );

        // Select all checkbox
        $('#selectAll').on('change', function () {
            $('.row-checkbox').prop('checked', $(this).prop('checked'));
        });

        // Individual checkbox selection
        $('.row-checkbox').on('change', function () {
            if (!$(this).prop('checked')) {
                $('#selectAll').prop('checked', false);
            }
        });

        // Sortable table headers
        $('.sortable').on('click', function () {
            var $this = $(this);
            var column = $this.data('column');
            var order = $this.hasClass('asc') ? 'desc' : 'asc';

            $('.sortable').removeClass('asc desc');
            $this.addClass(order);

            // Add sort icon
            $('.sortable i').remove();
            var icon = order === 'asc' ? 'fa-sort-up' : 'fa-sort-down';
            $this.append(' <i class="fas ' + icon + '"></i>');

            // Call sort function (implement based on your needs)
            sortTable(column, order);
        });
    }

    /* ============================================
       FORM VALIDATION
       ============================================ */
    function initializeFormValidation() {
        // Real-time validation
        $('.admin-form input, .admin-form select, .admin-form textarea').on('blur', function () {
            validateField($(this));
        });

        // Form submit validation
        $('.admin-form').on('submit', function (e) {
            var isValid = true;
            $(this).find('input:not([type="hidden"]), select, textarea').each(function () {
                if (!validateField($(this))) {
                    isValid = false;
                }
            });

            if (!isValid) {
                e.preventDefault();
                showAlert('danger', 'Vui lòng kiểm tra lại thông tin!');
                return false;
            }
        });

        // Auto-format numbers
        $('input[type="number"]').on('input', function () {
            var value = $(this).val();
            if (value && !isNaN(value)) {
                // Format number with commas (optional)
                // $(this).val(formatNumber(value));
            }
        });
    }

    function validateField($field) {
        var value = $field.val();
        var isValid = true;
        var errorMsg = '';

        // Required validation
        if ($field.prop('required') && !value) {
            isValid = false;
            errorMsg = 'Trường này là bắt buộc';
        }

        // Email validation
        if ($field.attr('type') === 'email' && value) {
            var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(value)) {
                isValid = false;
                errorMsg = 'Email không hợp lệ';
            }
        }

        // Phone validation
        if ($field.attr('type') === 'tel' && value) {
            var phoneRegex = /^[0-9]{10,11}$/;
            if (!phoneRegex.test(value.replace(/\s/g, ''))) {
                isValid = false;
                errorMsg = 'Số điện thoại không hợp lệ';
            }
        }

        // Display validation result
        var $feedback = $field.next('.invalid-feedback');
        if (!$feedback.length) {
            $feedback = $('<div class="invalid-feedback"></div>');
            $field.after($feedback);
        }

        if (isValid) {
            $field.removeClass('is-invalid').addClass('is-valid');
            $feedback.hide();
        } else {
            $field.removeClass('is-valid').addClass('is-invalid');
            $feedback.text(errorMsg).show();
        }

        return isValid;
    }

    /* ============================================
       CONFIRM DIALOGS
       ============================================ */
    function initializeConfirmDialogs() {
        // Delete confirmation
        $('a[href*="Delete"], button[type="submit"]').on('click', function (e) {
            var $this = $(this);
            var text = $this.text().trim().toLowerCase();

            if (text.includes('xóa') || text.includes('delete') || $this.hasClass('btn-danger')) {
                e.preventDefault();

                if (confirm('Bạn có chắc chắn muốn xóa? Hành động này không thể hoàn tác!')) {
                    if ($this.is('a')) {
                        window.location.href = $this.attr('href');
                    } else {
                        $this.closest('form').submit();
                    }
                }
            }
        });

        // Custom confirm dialog
        $('.confirm-action').on('click', function (e) {
            e.preventDefault();
            var message = $(this).data('confirm-message') || 'Bạn có chắc chắn?';

            if (confirm(message)) {
                window.location.href = $(this).attr('href');
            }
        });
    }

    /* ============================================
       TOOLTIPS & POPOVERS
       ============================================ */
    function initializeTooltips() {
        // Bootstrap tooltips
        $('[data-toggle="tooltip"], [data-bs-toggle="tooltip"]').tooltip();

        // Bootstrap popovers
        $('[data-toggle="popover"], [data-bs-toggle="popover"]').popover();

        // Custom tooltips for table actions
        $('.table-actions a, .table-actions button').each(function () {
            var title = $(this).attr('title') || $(this).text().trim();
            $(this).attr('data-toggle', 'tooltip').attr('title', title);
        });
    }

    /* ============================================
       ANIMATIONS
       ============================================ */
    function initializeAnimations() {
        // Scroll animations
        $(window).on('scroll', function () {
            $('.fade-in-scroll').each(function () {
                var elementTop = $(this).offset().top;
                var viewportBottom = $(window).scrollTop() + $(window).height();

                if (elementTop < viewportBottom - 50) {
                    $(this).addClass('visible');
                }
            });
        });

        // Counter animation for stats
        $('.stat-number').each(function () {
            var $this = $(this);
            var countTo = parseInt($this.text().replace(/[^0-9]/g, ''));

            if (!isNaN(countTo)) {
                $({ countNum: 0 }).animate(
                    { countNum: countTo },
                    {
                        duration: 2000,
                        easing: 'swing',
                        step: function () {
                            $this.text(Math.floor(this.countNum).toLocaleString('vi-VN'));
                        },
                        complete: function () {
                            $this.text(countTo.toLocaleString('vi-VN'));
                        }
                    }
                );
            }
        });
    }

    /* ============================================
       IMAGE PREVIEW
       ============================================ */
    function initializeImagePreviews() {
        $('input[type="file"]').on('change', function (e) {
            var file = e.target.files[0];
            if (file && file.type.startsWith('image/')) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    var $preview = $('#imagePreview');
                    if (!$preview.length) {
                        $preview = $('<img id="imagePreview" class="img-thumbnail mt-2" style="max-width: 200px;">');
                        $(this).after($preview);
                    }
                    $preview.attr('src', e.target.result).show();
                }.bind(this);
                reader.readAsDataURL(file);
            }
        });
    }

    /* ============================================
       SEARCH FUNCTIONALITY
       ============================================ */
    function initializeSearch() {
        var searchTimeout;

        $('#searchInput, .search-box input').on('keyup', function () {
            clearTimeout(searchTimeout);
            var searchTerm = $(this).val().toLowerCase();

            searchTimeout = setTimeout(function () {
                filterTable(searchTerm);
            }, 300);
        });
    }

    function filterTable(searchTerm) {
        $('.admin-table tbody tr').each(function () {
            var rowText = $(this).text().toLowerCase();
            if (rowText.indexOf(searchTerm) > -1) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });

        // Show "No results" message if needed
        var visibleRows = $('.admin-table tbody tr:visible').length;
        if (visibleRows === 0) {
            if (!$('#noResults').length) {
                $('.admin-table tbody').append(
                    '<tr id="noResults"><td colspan="100%" class="text-center text-muted">Không tìm thấy kết quả</td></tr>'
                );
            }
        } else {
            $('#noResults').remove();
        }
    }

    /* ============================================
       FILTERS
       ============================================ */
    function initializeFilters() {
        $('.filter-select').on('change', function () {
            var filterType = $(this).data('filter-type');
            var filterValue = $(this).val();

            applyFilter(filterType, filterValue);
        });

        // Date range filter
        $('#dateFrom, #dateTo').on('change', function () {
            var dateFrom = $('#dateFrom').val();
            var dateTo = $('#dateTo').val();

            if (dateFrom && dateTo) {
                filterByDateRange(dateFrom, dateTo);
            }
        });

        // Clear filters
        $('#clearFilters').on('click', function () {
            $('.filter-select').val('');
            $('#dateFrom, #dateTo').val('');
            $('.admin-table tbody tr').show();
        });
    }

    function applyFilter(type, value) {
        if (!value) {
            $('.admin-table tbody tr').show();
            return;
        }

        $('.admin-table tbody tr').each(function () {
            var $row = $(this);
            var cellValue = $row.find('[data-' + type + ']').data(type);

            if (cellValue == value) {
                $row.show();
            } else {
                $row.hide();
            }
        });
    }


    /* ============================================
       ALERTS
       ============================================ */
    function showAlert(type, message, duration) {
        duration = duration || 3000;

        var alertClass = 'alert-' + type;
        var iconClass = {
            'success': 'fa-check-circle',
            'danger': 'fa-exclamation-circle',
            'warning': 'fa-exclamation-triangle',
            'info': 'fa-info-circle'
        }[type] || 'fa-info-circle';

        var $alert = $(
            '<div class="alert admin-alert ' + alertClass + ' alert-dismissible fade show" role="alert">' +
            '<i class="fas ' + iconClass + ' alert-icon"></i>' +
            '<span>' + message + '</span>' +
            '<button type="button" class="btn-close" data-bs-dismiss="alert"></button>' +
            '</div>'
        );

        $('.container').first().prepend($alert);

        setTimeout(function () {
            $alert.fadeOut(function () {
                $(this).remove();
            });
        }, duration);
    }

    /* ============================================
       UTILITY FUNCTIONS
       ============================================ */
    function formatNumber(num) {
        return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    }

    function formatCurrency(amount) {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(amount);
    }

    function sortTable(column, order) {
        // Implement table sorting logic based on your needs
        console.log('Sort by', column, order);
    }

    /* ============================================
       EXPORT TO GLOBAL
       ============================================ */
    window.AdminJS = {
        showLoading: showLoading,
        hideLoading: hideLoading,
        showAlert: showAlert,
        formatNumber: formatNumber,
        formatCurrency: formatCurrency
    };

    /* ============================================
       KEYBOARD SHORTCUTS
       ============================================ */
    $(document).on('keydown', function (e) {
        // Ctrl + S to save form
        if ((e.ctrlKey || e.metaKey) && e.keyCode == 83) {
            e.preventDefault();
            $('form .btn-primary[type="submit"]').click();
        }

        // Escape to close modals
        if (e.keyCode == 27) {
            $('.modal').modal('hide');
        }
    });

    /* ============================================
       AUTO-SAVE DRAFT (Optional)
       ============================================ */
    var autoSaveInterval;
    function startAutoSave() {
        autoSaveInterval = setInterval(function () {
            var formData = $('.admin-form').serialize();
            localStorage.setItem('formDraft', formData);
            console.log('Draft saved');
        }, 30000); // Save every 30 seconds
    }

    function loadDraft() {
        var draft = localStorage.getItem('formDraft');
        if (draft && confirm('Bạn có muốn khôi phục bản nháp?')) {
            // Parse and populate form
            console.log('Draft loaded');
        }
    }

})(jQuery);