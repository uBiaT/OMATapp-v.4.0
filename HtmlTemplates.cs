namespace ShopeeServer
{
    public static class HtmlTemplates
    {
        public static string Index = @"
<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no'>
    <title>Shopee WMS Pro</title>
    <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.0/font/bootstrap-icons.css'>
    <script src='https://unpkg.com/vue@3/dist/vue.global.js'></script>
    <style>
        body { background-color: #f4f6f8; padding-bottom: 100px; font-size: 14px; font-family: -apple-system, sans-serif; }
        
        /* Card Đơn hàng */
        .card-item { background: white; border-radius: 8px; box-shadow: 0 1px 3px rgba(0,0,0,0.05); margin-bottom: 10px; border: 1px solid #eee; overflow: hidden; }
        .card-body-custom { padding: 12px; display: flex; align-items: flex-start; position: relative; }
        
        .img-box { position: relative; width: 90px; height: 90px; flex-shrink: 0; margin-right: 12px; cursor: pointer; }
        .img-thumb { width: 100%; height: 100%; object-fit: cover; border-radius: 6px; border: 1px solid #eee; }
        .zoom-icon { position: absolute; bottom: 0; right: 0; background: rgba(0,0,0,0.6); color: white; font-size: 10px; padding: 2px 4px; border-radius: 4px 0 4px 0; }
        
        .info-box { flex-grow: 1; min-width: 0; display: flex; flex-direction: column; }
        .product-name { font-weight: 700; color: #222; margin-bottom: 4px; line-height: 1.3; font-size: 13px; display: -webkit-box; -webkit-line-clamp: 2; -webkit-box-orient: vertical; overflow: hidden; }
        .variation-badge { font-size: 12px; color: #e65100; background: #fff3e0; border: 1px solid #ffe0b2; padding: 2px 6px; border-radius: 4px; width: fit-content; margin-bottom: 4px; font-weight: 500; }
        .location-badge { font-size: 11px; color: #1565c0; background: #e3f2fd; padding: 2px 8px; border-radius: 4px; width: fit-content; font-weight: bold; border: 1px solid #bbdefb; margin-top: 2px; }
        
        .qty-box { text-align: right; padding-left: 8px; display: flex; flex-direction: column; align-items: flex-end; min-width: 40px; }
        .qty-text { font-size: 25px; font-weight: 700; color: #333; }
        .qty-text.red { color: #d32f2f; }
        
        /* Header đơn hàng */
        .order-header { padding: 12px; background: white; border-bottom: 1px solid #f0f0f0; cursor: pointer; display: flex; justify-content: space-between; align-items: center; border-radius: 8px; margin-bottom: 8px; box-shadow: 0 1px 2px rgba(0,0,0,0.05); }
        .order-header.active { background: #e8f5e9; border: 1px solid #81c784; margin-bottom: 0; border-radius: 8px 8px 0 0; }
        .highlight-sn { color: #d32f2f; font-weight: 900; background: #ffebee; padding: 0 2px; border-radius: 2px; }
        .order-detail-box { background: #fafafa; border: 1px solid #81c784; border-top: none; border-radius: 0 0 8px 8px; padding: 10px; margin-bottom: 10px; }
        
        /* Console Log */
        .console-box { background: #1e1e1e; color: #00ff00; font-family: monospace; padding: 10px; border-radius: 6px; height: 400px; overflow-y: auto; font-size: 12px; border: 1px solid #444; }
        .log-line { border-bottom: 1px solid #333; padding: 2px 0; white-space: pre-wrap; word-break: break-all; }

        /* Picking UI */
        .picking-group-header { background: #ff9800; color: white; padding: 8px 12px; font-weight: bold; border-radius: 6px; margin-top: 15px; margin-bottom: 8px; display: flex; justify-content: space-between; }
        .picking-card.done { opacity: 0.4; filter: grayscale(100%); }
        .big-checkbox { width: 24px; height: 24px; accent-color: #2e7d32; margin-top: 2px; }
        
        /* Modal Style */
        .modal-main-img { width: 100%; height: 200px; object-fit: contain; background: #fff; }
        .comp-item { display: flex; align-items: center; padding: 8px; border-bottom: 1px solid #eee; cursor: pointer; }
        .comp-item:hover { background: #f9f9f9; }
        .comp-img { width: 40px; height: 40px; border-radius: 4px; border: 1px solid #ddd; margin-right: 10px; object-fit: cover; }
    </style>
</head>
<body>
<div id='app' class='container py-2' style='max-width:600px'>
    
    <div class='d-flex justify-content-between align-items-center mb-3 bg-white p-3 rounded shadow-sm sticky-top'>
        <div class='d-flex flex-column'>
            <span class='fw-bold text-primary h5 mb-0'>KHO HÀNG</span>
            <div class='d-flex align-items-center mt-1'>
                <i class='bi bi-zoom-out text-muted small me-1'></i>
                <input type='range' class='form-range' style='width:90px' min='0.5' max='1.5' step='0.05' v-model='zoomLevel' @input='updateZoom'>
                <i class='bi bi-zoom-in text-muted small ms-1'></i>
            </div>
        </div>
        
        <div class='btn-group'>
            <button class='btn btn-sm' :class='currentView==""manager"" ? ""btn-primary"" : ""btn-outline-primary""' @click='currentView=""manager""'>Đơn hàng</button>
            <button class='btn btn-sm' :class='currentView==""logs"" ? ""btn-primary"" : ""btn-outline-primary""' @click='fetchLogs(); currentView=""logs""'>Hệ thống</button>
        </div>
    </div>

    <div v-if='currentView === ""manager""'>
        <div v-if='!hasToken' class='alert alert-danger mb-3 shadow-sm'>
            <i class='bi bi-exclamation-triangle-fill'></i> Chưa kết nối Shopee. Vào Tab <b>Hệ thống</b> để đăng nhập ngay!
        </div>

        <ul class='nav nav-pills nav-fill mb-3 bg-white p-1 rounded shadow-sm'>
            <li class='nav-item'><a class='nav-link' :class='{active: tab===""unprocessed""}' @click='tab=""unprocessed""'>Chờ xử lý ({{unprocessedOrders.length}})</a></li>
            <li class='nav-item'><a class='nav-link' :class='{active: tab===""processed""}' @click='tab=""processed""'>Đã xử lý ({{processedOrders.length}})</a></li>
        </ul>

        <div class='d-flex justify-content-between mb-2 align-items-center' v-if='tab===""unprocessed""'>
            <div>
                <button class='btn btn-sm btn-light border shadow-sm' @click='sortDesc = !sortDesc'>
                    <i class='bi' :class='sortDesc ? ""bi-sort-down"" : ""bi-sort-up""'></i> 
                    {{ sortDesc ? 'Mới nhất' : 'Cũ nhất' }}
                </button>
                <button class='btn btn-sm btn-light border shadow-sm ms-1' @click='fetchData' title='Tải lại'>
                    <i class='bi bi-arrow-clockwise'></i>
                </button>
            </div>
            
            <button v-if='isBatchMode && selectedCount > 0' class='btn btn-sm btn-success fw-bold shadow-sm' @click='startPicking'>
                Đi nhặt ({{selectedCount}})<i class='bi bi-arrow-right'></i>
            </button>
            <button class='btn btn-sm shadow-sm fw-bold me-2' :class='isBatchMode ? ""btn-danger"" : ""btn-warning""' @click='toggleBatchMode'>
                {{ isBatchMode ? '❌ Hủy' : '📦 Gom' }}
            </button>
        </div>

        <div v-for='order in filteredOrders' :key='order.OrderId'>
            <div class='order-header' :class='{active: openOrderId === order.OrderId}' @click='toggleOrder(order.OrderId)'>
                <div class='d-flex align-items-center'>
                    <input v-if='isBatchMode' type='checkbox' class='form-check-input me-3 big-checkbox' v-model='order.Selected' @click.stop>
                    <div>
                        <span style='font-family:monospace;font-size:1.1em'>#{{order.OrderId.slice(0, -4)}}<span class='highlight-sn'>{{order.OrderId.slice(-4)}}</span></span>
                        <div class='small text-muted'>{{ formatTime(order.CreatedAt) }}</div>
                    </div>
                </div>
                <span class='badge bg-secondary rounded-pill'>{{order.Items.length}} món</span>
            </div>

            <div v-if='openOrderId === order.OrderId' class='order-detail-box'>
                <div v-for='item in order.Items' class='card-item border-0 mb-1'>
                    <div class='card-body-custom'>
                        <div class='img-box' @click.stop='showProductModal(item)'>
                            <img :src='item.ImageUrl' class='img-thumb'>
                            <div class='zoom-icon'><i class='bi bi-eye-fill'></i></div>
                        </div>
                        <div class='info-box'>
                            <div class='product-name'>{{item.ProductName}}</div>
                            <div class='variation-badge'>{{item.ModelName}}</div>
                            <div v-if='item.Shelf' class='location-badge'>
                                <i class='bi bi-geo-alt-fill'></i> {{item.Shelf}}{{item.Level}}
                            </div>
                        </div>
                        <div class='qty-box'><span class='qty-text' :class='{red: item.Quantity > 1}'>x{{item.Quantity}}</span></div>
                    </div>
                </div>
                <button v-if='!isBatchMode' class='btn btn-danger w-100 mt-2 fw-bold py-2' @click='shipOrder(order.OrderId)'>
                    <i class='bi bi-printer-fill'></i> CHUẨN BỊ ĐƠN & IN
                </button>
            </div>
        </div>
    </div>

    <div v-if='currentView === ""picking""'>
        <div class='sticky-top bg-white p-3 shadow-sm d-flex justify-content-between mb-3 align-items-center'>
            <button class='btn btn-outline-secondary btn-sm' @click='currentView=""manager""'>Quay lại</button>
            <span class='fw-bold fs-5'>
                Đã nhặt: <span class='text-success'>{{ pickedQty }}</span> / {{ totalQty }}
            </span>
        </div>
        
        <div class='progress fixed-top' style='height: 5px; z-index: 2000;'>
            <div class='progress-bar bg-success' :style='{width: (pickedQty / totalQty * 100) + ""%""}'></div>
        </div>

        <div v-for='(group, loc) in groupedBatch' :key='loc'>
            <div class='picking-group-header'>
                <span><i class='bi bi-geo-alt-fill'></i> {{loc}}</span>
                <span class='badge bg-white text-dark'>{{group.length}} loại</span>
            </div>
            
            <div v-for='item in group' class='card-item picking-card' :class='{done: item.Picked}'>
                <div class='card-body-custom'>
                    <div class='img-box' @click.stop='showProductModal(item)'>
                        <img :src='item.ImageUrl' class='img-thumb'>
                    </div>
                    <div class='info-box'>
                        <div class='product-name'>{{item.ProductName}}</div>
                        <div class='variation-badge'>{{item.ModelName}}</div>
                        <div class='small text-muted mt-1'>
                            Đơn: <span v-for='id in item.OrderIds' class='badge bg-light text-dark border me-1'>{{id}}</span>
                        </div>
                    </div>
                    <div class='qty-box'>
                        <span class='qty-text' :class='{red: item.TotalQty > 1}'>{{item.TotalQty}}</span>
                        <input type='checkbox' class='big-checkbox mt-2' v-model='item.Picked'>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div v-if='currentView === ""logs""'>
        <div class='card-item p-3 mb-3 border-warning border-2'>
            <h5><i class='bi bi-key-fill'></i> Kết nối Shopee</h5>
            <div v-if='hasToken' class='text-success fw-bold mb-2'><i class='bi bi-check-circle-fill'></i> Đã kết nối</div>
            <div v-else class='text-danger fw-bold mb-2'><i class='bi bi-x-circle-fill'></i> Chưa kết nối</div>
            <hr>
            <a :href='loginUrl' target='_blank' class='btn btn-warning w-100 mb-2 fw-bold'>Bước 1: Lấy Link Đăng Nhập</a>
            <textarea v-model='callbackUrl' class='form-control mb-2' rows='3' placeholder='Bước 2: Dán link kết quả (Callback URL) vào đây...'></textarea>
            <button class='btn btn-primary w-100' @click='doLogin' :disabled='!callbackUrl'>Lưu Kết Nối</button>
        </div>

        <div class='card-item p-3'>
            <div class='d-flex justify-content-between mb-2'>
                <h5 class='text-dark'><i class='bi bi-terminal-fill'></i> Server Logs</h5>
                <button class='btn btn-sm btn-light border' @click='fetchLogs'><i class='bi bi-arrow-clockwise'></i></button>
            </div>
            <div class='console-box'>
                <div v-for='line in logs' class='log-line'>{{line}}</div>
            </div>
        </div>
    </div>

    <div class='modal fade' id='productModal' tabindex='-1'>
        <div class='modal-dialog modal-dialog-centered'>
            <div class='modal-content'>
                <div class='modal-header border-0 pb-0'>
                    <h6 class='modal-title fw-bold text-primary'>CHI TIẾT & TỒN KHO</h6>
                    <button type='button' class='btn-close' data-bs-dismiss='modal'></button>
                </div>
                <div class='modal-body pt-2'>
                    <div v-if='loadingModal' class='text-center py-5 text-warning fw-bold'>
                        <div class='spinner-border'></div><br>Đang tải dữ liệu...
                    </div>
                    <div v-else>
                        <div class='text-center mb-3'>
                            <img :src='modalItem.img' class='modal-main-img rounded border mb-2'>
                            <h6 class='fw-bold text-dark px-2'>{{modalItem.name}}</h6>
                            <div class='badge bg-success fs-5 p-2 mt-1'>Kho: {{modalItem.stock}}</div>
                        </div>
                        <div class='card bg-light border-0'>
                            <div class='card-header bg-transparent fw-bold small text-muted'>PHÂN LOẠI KHÁC</div>
                            <div class='card-body p-0' style='max-height:200px;overflow-y:auto'>
                                <div v-for='v in variations' class='comp-item' @click='selectVariation(v)'>
                                    <img :src='v.img' class='comp-img'>
                                    <div class='flex-grow-1 small fw-bold'>{{v.name}}</div>
                                    <div class='fw-bold text-success'>{{v.stock}}</div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</div>

<script src='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js'></script>
<script>
    const { createApp } = Vue;
    createApp({
        data() {
            return {
                orders: [], tab: 'unprocessed', currentView: 'manager', 
                sortDesc: false, 
                logs: [], hasToken: false, loginUrl: '', callbackUrl: '',
                isBatchMode: false, batchItems: [], 
                openOrderId: null, 
                loadingModal: false, modalItem: {}, variations: [],
                zoomLevel: 1.0 // Biến Zoom
            }
        },
        computed: {
            unprocessedOrders() { return this.orders.filter(o => o.Status === 0); },
            processedOrders() { return this.orders.filter(o => o.Status === 1); },
            filteredOrders() { 
                let list = this.tab === 'unprocessed' ? this.unprocessedOrders : this.processedOrders;
                return [...list].sort((a, b) => this.sortDesc ? b.CreatedAt - a.CreatedAt : a.CreatedAt - b.CreatedAt);
            },
            selectedCount() { return this.orders.filter(o => o.Selected).length; },
            groupedBatch() {
                const groups = {};
                this.batchItems.forEach(i => {
                    const key = i.Location || 'Khác';
                    if(!groups[key]) groups[key] = [];
                    groups[key].push(i);
                });
                return Object.keys(groups).sort().reduce((obj, key) => { obj[key] = groups[key]; return obj; }, {});
            },
            totalQty() { return this.batchItems.reduce((s, i) => s + i.TotalQty, 0); },
            pickedQty() { return this.batchItems.reduce((s, i) => s + (i.Picked ? i.TotalQty : 0), 0); }
        },
        mounted() {
            this.fetchData();
            this.updateZoom(); // Load zoom mặc định
            setInterval(this.fetchData, 5000);
            setInterval(this.fetchLogs, 3000);
        },
        methods: {
            updateZoom() { document.body.style.zoom = this.zoomLevel; },
            formatTime(ts) { return new Date(ts * 1000).toLocaleString('vi-VN'); },
            async fetchData() {
                try {
                    const res = await fetch('/api/data');
                    const data = await res.json();
                    this.hasToken = data.hasToken;
                    this.loginUrl = data.loginUrl;
                    const selectedIds = new Set(this.orders.filter(o => o.Selected).map(o => o.OrderId));
                    if(JSON.stringify(this.orders.map(o=>o.OrderId)) !== JSON.stringify(data.orders.map(o=>o.OrderId))) {
                         this.orders = data.orders.map(o => ({...o, Selected: selectedIds.has(o.OrderId)}));
                    }
                } catch(e) {}
            },
            async fetchLogs() {
                if(this.currentView !== 'logs') return;
                try {
                    const res = await fetch('/api/logs');
                    this.logs = await res.json();
                } catch(e) {}
            },
            async doLogin() {
                if(!this.callbackUrl) return;
                try {
                    const res = await fetch('/api/login', {
                        method: 'POST',
                        body: JSON.stringify({url: this.callbackUrl})
                    });
                    const d = await res.json();
                    if(d.success) {
                        alert('Đăng nhập thành công!');
                        this.callbackUrl = '';
                        this.fetchLogs();
                    } else { alert('Lỗi: ' + (d.message || 'Link sai')); }
                } catch(e) { alert('Lỗi mạng'); }
            },
            toggleBatchMode() { 
                this.isBatchMode = !this.isBatchMode; 
                this.orders.forEach(o => o.Selected = false); 
                this.openOrderId = null;
            },
            toggleOrder(id) {
                if (this.isBatchMode) { 
                    const o = this.orders.find(x => x.OrderId === id); 
                    if(o) o.Selected = !o.Selected; 
                } else { this.openOrderId = (this.openOrderId === id) ? null : id; }
            },
            async shipOrder(id) {
                let nextId = null;
                const currentIdx = this.filteredOrders.findIndex(o => o.OrderId === id);
                if (currentIdx !== -1 && currentIdx + 1 < this.filteredOrders.length) {
                    nextId = this.filteredOrders[currentIdx + 1].OrderId;
                }
                await fetch(`/api/ship?id=${id}`, {method: 'POST'});
                const o = this.orders.find(x => x.OrderId === id);
                if(o) o.Status = 1;
                if (nextId) this.openOrderId = nextId; else this.openOrderId = null;
            },
            startPicking() {
                const selected = this.orders.filter(o => o.Selected);
                if(selected.length === 0) return;
                const agg = {};
                selected.forEach(order => {
                    order.Items.forEach(item => {
                        const key = item.ModelName; 
                        if(!agg[key]) agg[key] = { 
                            ProductName: item.ProductName, ModelName: item.ModelName, 
                            ImageUrl: item.ImageUrl, Location: item.Shelf || 'Chưa định vị', 
                            TotalQty: 0, OrderIds: [], Picked: false 
                        };
                        agg[key].TotalQty += item.Quantity;
                        agg[key].OrderIds.push(order.OrderId.slice(-4));
                    });
                });
                this.batchItems = Object.values(agg);
                this.currentView = 'picking';
            },
            async showProductModal(item) {
                this.loadingModal = true;
                this.modalItem = { name: item.ModelName, img: item.ImageUrl, stock: '...' };
                this.variations = [];
                new bootstrap.Modal(document.getElementById('productModal')).show();
                try {
                    const res = await fetch('/api/product?id=' + item.ItemId);
                    const data = await res.json();
                    if(data.success) {
                        this.variations = data.variations;
                        const current = this.variations.find(v => v.name === item.ModelName);
                        if(current) this.modalItem = { ...current, name: current.name, img: current.img, stock: current.stock };
                    }
                } catch(e) {}
                this.loadingModal = false;
            },
            selectVariation(v) { this.modalItem = { name: v.name, img: v.img, stock: v.stock }; }
        }
    }).mount('#app');
</script>
</body>
</html>";
    }
}