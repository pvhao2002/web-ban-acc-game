DROP DATABASE hpshop;
GO
CREATE DATABASE hpshop;
GO
USE hpshop;
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
    description   NVARCHAR(MAX),
    status        VARCHAR(50) DEFAULT 'active',
    created_at    DATETIME    DEFAULT GETDATE(),
    updated_at    DATETIME    DEFAULT GETDATE()
);
GO
CREATE TABLE products
(
    product_id    INT IDENTITY PRIMARY KEY,
    product_name  NVARCHAR(255),
    price         DECIMAL(10, 2),
    stock         INT         DEFAULT 0,
    description   NVARCHAR(MAX),
    product_image NVARCHAR(2000),
    category_id   INT,
    status        VARCHAR(50) DEFAULT 'active',
    sold          INT         DEFAULT 0,
    created_at    DATETIME    DEFAULT GETDATE(),
    updated_at    DATETIME    DEFAULT GETDATE(),
    FOREIGN KEY (category_id) REFERENCES categories (category_id)
);
GO
CREATE TABLE rating
(
    rate_id    INT IDENTITY PRIMARY KEY,
    user_id    INT,
    product_id INT,
    rate       INT,
    FOREIGN KEY (user_id) REFERENCES users (user_id),
    FOREIGN KEY (product_id) REFERENCES products (product_id)
);
GO
CREATE TABLE comment
(
    comment_id   INT IDENTITY PRIMARY KEY,
    user_id      INT,
    product_id   INT,
    content      NVARCHAR(MAX),
    comment_date DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES users (user_id),
    FOREIGN KEY (product_id) REFERENCES products (product_id)
);
GO
CREATE TABLE carts
(
    cart_id        INT IDENTITY PRIMARY KEY,
    user_id        INT,
    total_price    DECIMAL(10, 2),
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
    total_price  DECIMAL(10, 2),
    created_at   DATETIME DEFAULT GETDATE(),
    updated_at   DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (cart_id) REFERENCES carts (cart_id),
    FOREIGN KEY (product_id) REFERENCES products (product_id)
);
GO
CREATE TABLE orders
(
    order_id         INT IDENTITY PRIMARY KEY,
    user_id          INT,
    shipping_address NVARCHAR(255),
    phone_number     NVARCHAR(20),
    full_name        NVARCHAR(255),
    total_price      DECIMAL(10, 2),
    total_quantity   INT,
    status           VARCHAR(50) DEFAULT 'pending',
    created_at       DATETIME    DEFAULT GETDATE(),
    updated_at       DATETIME    DEFAULT GETDATE(),
    FOREIGN KEY (user_id) REFERENCES users (user_id)
);
GO
CREATE TABLE order_items
(
    order_item_id INT IDENTITY PRIMARY KEY,
    order_id      INT,
    product_id    INT,
    quantity      INT,
    total_price   DECIMAL(10, 2),
    created_at    DATETIME DEFAULT GETDATE(),
    updated_at    DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (order_id) REFERENCES orders (order_id),
    FOREIGN KEY (product_id) REFERENCES products (product_id)
);
GO
CREATE TABLE about
(
    about_id INT IDENTITY PRIMARY KEY,
    content  NVARCHAR(MAX)
);
GO
CREATE TABLE slider
(
    slider_id  INT IDENTITY PRIMARY KEY,
    slider_img NVARCHAR(MAX),
    position   INT
);
GO
CREATE TABLE policy
(
    policy_id      INT IDENTITY PRIMARY KEY,
    policy_content NVARCHAR(MAX)
);


GO

-- INSERT DATA FOR TABLE users
INSERT INTO users (email, password, full_name, role, status)
VALUES ('admin@gmail.com',
        '1234qwer',
        N'Quản trị viên',
        'admin',
        'active');
INSERT INTO users (email,
                   password,
                   full_name,
                   role,
                   status)
VALUES ('user1@example.com',
        '1234qwer',
        'User 1',
        'user',
        'active'),
       ('user2@example.com',
        '1234qwer',
        'User 2',
        'user',
        'active');
-- INSERT DATA FOR TABLE categories
INSERT INTO categories (category_name, description, status)
VALUES (N'Hành động',
        N'Hành động',
        'active'),
       (N'Phiêu lưu',
        N'Phiêu lưu',
        'active'),
       (N'Mô phỏng',
        N'Mô phỏng',
        'active');


insert into about(content)
values (N'<div class="container mt-3"><div class="row"><div class="col-12 col-sm-12 col-md-12 col-lg-12 main-col"><div class="text-center mb-4"><h2 class="h2"><mark class="marker-green">HP Shop</mark></h2><div class="rte-setting"><p>HP Shop được thành lập vào năm 2014, đánh dấu bước khởi đầu của thương hiệu tại Việt Nam và là một trong những cửa hàng đầu tiên được thành lập. Vào năm 2017, trong giai đoạn mở rộng ra thị trường chính hãng tại Việt Nam, HP Shop chính thức trở thành một trong những hệ thống phân phối chính thức qua sự hợp tác với DigiWorld. Nhờ đó, khách hàng có thể hoàn toàn yên tâm về chất lượng và nguồn gốc của các sản phẩm tại HP Shop. Đến nay, với hơn 500 mẫu laptop đến từ các thương hiệu hàng đầu thế giới được phân phối bởi Digiworld và các đối tác, từ các mẫu laptop cho đến phụ kiện đi kèm, HP Shop đã thành công trong việc đa dạng hóa sản phẩm, mang đến cho khách hàng những trải nghiệm mua sắm tốt nhất.</p></div></div></div></div><div class="row"><div class="col-12 col-sm-4 col-md-4 col-lg-4 mb-4"><img class="blur-up lazyload" style="object-fit:contain;" src="/assets/client/images/collection/home14-bags-sm-banner1.jpg" alt="About Us" width="300" height="150" data-src="/assets/client/images/collection/home14-bags-sm-banner1.jpg"></div><div class="col-12 col-sm-4 col-md-4 col-lg-4 mb-4"><img class="blur-up lazyload" style="object-fit:contain;" src="/assets/client/images/lookbook-10.jpg" alt="About Us" width="300" height="150" data-src="/assets/client/images/lookbook-10.jpg"></div><div class="col-12 col-sm-4 col-md-4 col-lg-4 mb-4"><img class="blur-up lazyload" style="object-fit:contain;" src="/assets/client/images/lookbook-3.jpg" alt="About Us" width="300" height="150" data-src="/assets/client/images/lookbook-3.jpg"></div></div><div class="row"><div class="col-12"><p>Một trong những điều truyền cảm hứng nhất về Smartband shop là sứ mệnh của chúng tôi: "Tạo ra trải nghiệm mua sắm tốt nhất, giúp mọi người trên thế giới sống tốt hơn". Điều này không chỉ là một câu nói, mà là cốt lõi của mọi quyết định chúng tôi đưa ra. Chúng tôi tin rằng mỗi sản phẩm, dịch vụ và quyết định của chúng tôi sẽ giúp mọi người trên thế giới sống tốt hơn. Đó là lý do tại sao chúng tôi không ngừng cải tiến và phát triển, để mang lại trải nghiệm mua sắm tốt nhất cho mọi người. Chúng tôi tin rằng mỗi sản phẩm, dịch vụ và quyết định của chúng tôi sẽ giúp mọi người trên thế giới sống tốt hơn. Đó là lý do tại sao chúng tôi không ngừng cải tiến và phát triển, để mang lại trải nghiệm mua sắm tốt nhất cho mọi người. Chúng tôi tin rằng mỗi sản phẩm, dịch vụ và quyết định của chúng tôi sẽ giúp mọi người trên thế giới sống tốt hơn. Đó là lý do tại sao chúng tôi không ngừng cải tiến và phát triển, để mang lại trải nghiệm mua sắm tốt nhất cho mọi người. Chúng tôi tin rằng mỗi sản phẩm, dịch vụ và quyết định của chúng tôi sẽ giúp mọi người trên thế giới sống tốt hơn. Đó là lý do tại sao chúng tôi không ngừng cải tiến và phát triển, để mang lại trải nghiệm mua sắm tốt nhất cho mọi người.</p><p>+ Giờ bán hàng và nghe điện thoại : 8h30 đến 20h30 (tất cả các ngày trong tuần)</p><p>+ ĐT liên hệ: 1900.6429 (nhánh 7)</p><p>Chi nhánh Lĩnh Nam - HN</p><p>+ Thời gian phục vụ :</p><p>8h30 đến 12h00 / 13h đến 17h30 (t2 đến t7 / nghỉ CN)</p><p>+ ĐT liên hệ: 1900.6429 (nhánh 5)</p><p>Trả lời thắc mắc điện thoại, tin nhắn, coment trên fanpage facebook Cửa Hàng TCS</p><p>+ Thời gian phục vụ : 8h30 đến 20h30 (tất cả các ngày trong tuần)</p><p>Trả lời thắc mắc tin nhắn trên các kênh hợp tác shopee và lzd</p></div></div><div class="row"><div class="col-12 col-sm-12 col-md-6 col-lg-6 mb-4"><h2 class="h2">ĐỊNH HƯỚNG CỦA CHÚNG TÔI</h2><div class="rte-setting"><p>Chúng tôi tin rằng sự tư vấn, giao tiếp và dịch vụ xuất sắc dành cho khách hàng sẽ là nền tảng cho một kết quả tài chính vững chắc và phát triển bền vững. Laptop World đặt ra các tiêu chí phát triển công ty trên bốn phương diện chính:</p><ol><li><strong>Hiệu quả Quản lý Lãnh đạo</strong>: Chúng tôi nỗ lực tuyển chọn, phát triển và giữ chân những nhân viên xuất sắc từ mọi bộ phận để thúc đẩy sự đổi mới và phát triển chuyên môn. Chúng tôi chú trọng vào việc xây dựng một môi trường làm việc chuyên nghiệp, đa dạng và hòa nhập, cũng như không ngừng tái cơ cấu quản lý và chia sẻ kinh nghiệm nhằm tạo ra giá trị tốt nhất cho công việc.</li><li><strong>Tâm huyết với Nghề</strong>: Laptop World coi trọng việc lựa chọn và phát triển những cá nhân tài năng, đồng thời khen thưởng những đóng góp xuất sắc để khuyến khích sự tận tâm và đổi mới. Chúng tôi xây dựng chính sách đãi ngộ tốt nhất cho nhân sự gắn bó lâu dài, nhằm tạo điều kiện cho sự phát triển cá nhân và chuyên môn.</li><li><strong>Giá trị của Khách hàng</strong>: Tại Laptop World, chúng tôi đặt giá trị và sự hài lòng của khách hàng lên hàng đầu. Chúng tôi cam kết trở thành đối tác đáng tin cậy, mang đến những sản phẩm và dịch vụ tốt nhất, đồng thời duy trì mối quan hệ lâu dài với khách hàng thông qua sự tin tưởng và giá trị thiết thực.</li><li><strong>Điều hành Tăng trưởng Lợi nhuận</strong>: Chúng tôi luôn tìm kiếm cơ hội để học hỏi từ những kinh nghiệm và khám phá những ý tưởng mới. Mỗi ngày là một cơ hội để cải thiện và phát triển, điều này giúp định hình tương lai và sự phát triển lâu dài của Laptop World.</li></ol></div></div><div class="col-12 col-sm-12 col-md-6 col-lg-6"><h2 class="h2">Contact Us</h2><ul class="addressFooter"><li>55 Gallaxy Enque, 2568 steet, 23568 NY</li><li class="phone">(440) 000 000 0000</li><li class="email">sales@yousite.com</li></ul><hr><ul class="list--inline site-footer__social-icons social-icons"><li>&nbsp;</li><li><a class="social-icons__link" href="#" target="_blank" title="Belle Multipurpose Bootstrap 4 Template on Twitter"><span class="icon__fallback-text">Twitter</span>&nbsp;</a></li><li><a class="social-icons__link" href="#" target="_blank" title="Belle Multipurpose Bootstrap 4 Template on Pinterest"><span class="icon__fallback-text">Pinterest</span>&nbsp;</a></li><li><a class="social-icons__link" href="#" target="_blank" title="Belle Multipurpose Bootstrap 4 Template on Instagram"><span class="icon__fallback-text">Instagram</span>&nbsp;</a></li><li><a class="social-icons__link" href="#" target="_blank" title="Belle Multipurpose Bootstrap 4 Template on Tumblr"><span class="icon__fallback-text">Tumblr</span>&nbsp;</a></li><li><a class="social-icons__link" href="#" target="_blank" title="Belle Multipurpose Bootstrap 4 Template on YouTube"><span class="icon__fallback-text">YouTube</span>&nbsp;</a></li><li><a class="social-icons__link" href="#" target="_blank" title="Belle Multipurpose Bootstrap 4 Template on Vimeo"><span class="icon__fallback-text">Vimeo</span>&nbsp;</a></li></ul></div></div></div>');

insert into slider(slider_img, position)
values (N'~/Assets/client/BBQAx1N_d.webp', 1);
insert into slider(slider_img, position)
values (N'~/Assets/client/img1.webp', 2);
insert into slider(slider_img, position)
values (N'~/Assets/client/1.png', 3);

insert into policy(policy_content)
values (N'<h4>Chính sách:</h4>
                    <div class="policy-container">
                        <div class="policy-special">
                            <strong class="orange">1. Mua sản phẩm HP Shop được bảo hành như thế nào?</strong>
                            <div class="ask">
                                Để đảm bảo quyền lợi của quý khách hàng khi mua sản phẩm tại các cửa hàng thuộc hệ thống
                                cửa hàng HP Shop. Chúng tôi cam kết tất cả các sản phẩm được tuân theo các điều khoản
                                bảo hành của sản phẩm tại thời điểm xuất hóa đơn cho quý khách hàng. Các sản phẩm điện
                                thoại sẽ có chính sách bảo hành khác nhau tùy thuộc vào hãng sản xuất. Khách hàng có thể
                                bảo hành máy tại các cửa hàng HP Shop trên toàn quốc cũng như các trung tâm bảo hành
                                chính hãng sản phẩm.
                            </div>
                            <div class="ask">
                                Khách hàng có thể truy cập đường dẫn sau để tìm kiếm địa chỉ trung tâp bảo hoặc cửa hàng
                                HP Shop gần nhất
                            </div>
                            <div class="ask">
                                Để tra cứu thông tin máy gửi bảo hành, quý khách hàng có thể truy cập đường dẫn sau
                            </div>
                        </div>
                        <div class="policy-special">
                            <strong class="orange">
                                2. Mua sản phẩm tại HP Shop có được đổi trả không? Nếu được thì phí
                                đổi trả sẽ được tính như thế nào?
                            </strong>
                            <div class="ask">
                                Đối với các sản phẩm sản phẩm cùng model, cùng màu. Trong tình huống sản phẩm đổi hết hàng,
                                khách hàng có thể đổi sang một sản phẩm khác tương đương hoặc cao hơn về giá trị so với
                                sản phẩm lỗi. Trường hợp khách hàng muốn trả sản phẩm: HP Shop sẽ kiểm tra tình trạng
                                máy và thông báo đến Khách hàng về giá trị thu lại sản phẩm ngay tại cửa hàng.
                            </div>
                            <div class="ask">
                                Để biết thêm thông tin chi tiết, quý khách hàng truy cập đường dẫn bên dưới để nắm được
                                phí đổi trả chi tiết nhất.
                            </div>
                        </div>
                        <div class="policy-special">
                            <strong class="orange">
                                3. HP Shop có chính sách giao hàng tận nhà không? Nếu giao hàng tại nhà mà không ưng
                                sản phẩm có được trả lại không?
                            </strong>
                            <div class="ask">
                                HP Shop cam kết giao hàng toàn bộ 63 tỉnh thành, HP Shop nhận giao đơn hàng có thời
                                gian hẹn giao tại nhà trước 20h00. Miễn phí giao hàng với các đơn hàng trong bán kính
                                20km có đặt shop (Với đơn hàng có giá trị nhỏ hơn 100.000đ sẽ thu phí 10.000đ) nhân viên
                                HP Shop sẽ tư vấn chi tiết về cách thức giao nhận thuận tiện nhất.
                            </div>
                            <div class="ask">
                                Nếu quý khách hàng không ưng ý với sản phẩm khi nhận hàng, quý khách có thể
                                từ chối mua hàng mà không mất bất cứ chi phí nào. Để biết thêm thông tin, quý khách có
                                thể truy cập link bên dưới để biết thêm thông tin chi tiết:
                            </div>
                            <div class="ask des">Lưu ý:</div>
                            <div class="ask">
                                Đối với các sản phẩm còn nguyên seal, khách hàng muốn bóc seal sẽ phải thanh toán 100%
                                giá trị sản phẩm. Nếu không ưng, HP Shop sẽ hỗ trợ đổi sản phẩm cho khách hàng theo
                                chính sách đổi trả.
                            </div>
                            <div class="ask">
                                Đối với các sản phẩm không seal, quý khách hàng có thể kiểm tra máy mà không phải chịu
                                bất cứ chi phí nào nếu không mua.
                            </div>
                        </div>
                        <div class="policy-special">
                            <strong class="orange">4. Làm thế nào để được mua hàng theo chính sách F.Friends?</strong>
                            <div class="ask">
                                Để được mua hàng và hưởng quyền lợi theo chính sách mua hàng F.Friends, quý khách hàng
                                phải là hội viên. Để biết bạn có là hội viên hay không, bạn cần biết doanh nghiệp bạn
                                đang công tác đã ký hợp đồng tham gia chương trình F.Friends hay chưa. Điều kiện tiếp
                                theo là bạn đã ký hợp đồng chính thức với doanh nghiệp đó chưa.
                            </div>
                            <div class="ask">
                                Nếu bạn đã là hội viên của chương trình này, bạn sẽ được hưởng ưu đãi trả
                                thẳng giảm 3% khi mua sản phẩm. Để được trả góp bạn phải đủ 8 tháng công tác tại doanh
                                nghiệp.
                            </div>
                            <div class="ask">
                                Để biết thêm thông tin chi tiết. Quý khách vui lòng truy cập vào đường link
                                bên dưới
                            </div>
                        </div>
                    </div>');
