CREATE
DATABASE bshop;
GO
USE bshop;
GO
CREATE TABLE users
(
    user_id    INT IDENTITY PRIMARY KEY,
    email      NVARCHAR(255),
    password   NVARCHAR(255),
    full_name  NVARCHAR(255),
    role       VARCHAR(50) DEFAULT 'user',
    status     VARCHAR(50) DEFAULT 'active',
    created_at DATETIME    DEFAULT GETDATE(),
    updated_at DATETIME    DEFAULT GETDATE()
);
GO
CREATE TABLE categories
(
    category_id   INT IDENTITY PRIMARY KEY,
    category_name NVARCHAR(255),
    status        VARCHAR(50) DEFAULT 'active',
    created_at    DATETIME    DEFAULT GETDATE(),
    updated_at    DATETIME    DEFAULT GETDATE()
);
GO
CREATE TABLE products
(
    product_id    INT IDENTITY PRIMARY KEY,
    product_name  NVARCHAR(255),
    price         DECIMAL(25, 2),
    discount      DECIMAL(25, 2),
    description   NVARCHAR(MAX),
    product_image NVARCHAR(MAX),
    category_id   INT,
    status        VARCHAR(50) DEFAULT 'active',
    created_at    DATETIME    DEFAULT GETDATE(),
    updated_at    DATETIME    DEFAULT GETDATE(),
    FOREIGN KEY (category_id) REFERENCES categories (category_id)
);
GO
CREATE TABLE carts
(
    cart_id        INT IDENTITY PRIMARY KEY,
    user_id        INT,
    total_price    DECIMAL(25, 2),
    total_quantity INT,
    created_at     DATETIME DEFAULT GETDATE(),
    updated_at     DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES users (user_id)
);
GO
CREATE TABLE cart_items
(
    cart_item_id INT IDENTITY PRIMARY KEY,
    cart_id      INT,
    product_id   INT,
    quantity     INT,
    total_price  DECIMAL(25, 2),
    created_at   DATETIME DEFAULT GETDATE(),
    updated_at   DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (cart_id) REFERENCES carts (cart_id),
    FOREIGN KEY (product_id) REFERENCES products (product_id)
);
GO
CREATE TABLE orders
(
    order_id       INT IDENTITY PRIMARY KEY,
    user_id        INT,
    email          NVARCHAR(255),
    phone_number   NVARCHAR(20),
    full_name      NVARCHAR(255),
    total_price    DECIMAL(25, 2),
    total_quantity INT,
    status         VARCHAR(50) DEFAULT 'pending',
    payment_method VARCHAR(100),
    created_at     DATETIME    DEFAULT GETDATE(),
    updated_at     DATETIME    DEFAULT GETDATE(),
    tx_ref         NVARCHAR(555),
    FOREIGN KEY (user_id) REFERENCES users (user_id)
);
GO
CREATE TABLE order_items
(
    order_item_id INT IDENTITY PRIMARY KEY,
    order_id      INT,
    product_id    INT,
    quantity      INT,
    total_price   DECIMAL(25, 2),
    created_at    DATETIME DEFAULT GETDATE(),
    updated_at    DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (order_id) REFERENCES orders (order_id),
    FOREIGN KEY (product_id) REFERENCES products (product_id)
);
GO
-- INSERT DATA FOR TABLE users
INSERT INTO users (email, password, full_name, role, status)
VALUES ('admin@gmail.com',
        '1234qwer',
        'admin',
        'admin',
        'active');

INSERT INTO categories (category_name, status, created_at, updated_at)
VALUES (N'Board game classic', N'active', N'2024-09-29 08:13:26.713', N'2024-09-29 08:13:26.713');
INSERT INTO categories (category_name, status, created_at, updated_at)
VALUES (N'Board game family', N'active', N'2024-09-29 08:13:26.713', N'2024-09-29 08:13:26.713');
INSERT INTO categories (category_name, status, created_at, updated_at)
VALUES (N'Board game children', N'active', N'2024-09-29 08:13:26.713', N'2024-09-29 08:13:26.713');



