-- 创建数据库

CREATE DATABASE drug_system;

--  使用数据库

USE drug_system;

--  创建 custermor 表

CREATE TABLE Customer (
    Cid VARCHAR(20) PRIMARY KEY,                 -- 客户ID，VARCHAR类型，作为主键
    Cname VARCHAR(100) NOT NULL,                 -- 客户名，不能为空
    telephone VARCHAR(20) NOT NULL,              -- 电话号码，不能为空
    age INT NOT NULL CHECK (age > 0),            -- 年龄，不能为空，必须大于0
    province VARCHAR(100) NOT NULL,              -- 省份，不能为空
    city VARCHAR(100) NOT NULL,                  -- 城市，不能为空
    district VARCHAR(100) NOT NULL,              -- 区/县，不能为空
    detailed_address VARCHAR(255) NOT NULL,      -- 详细地址，不能为空
    email VARCHAR(100) NOT NULL,                 -- 电子邮件，不能为空
    UNIQUE (telephone),                          -- 电话号码唯一约束
    CHECK (CHAR_LENGTH(Cid) > 6)                 -- 客户ID长度必须大于6
);

--  创建 Supplier 表

CREATE TABLE Supplier (
    Sid VARCHAR(20) PRIMARY KEY,                 -- 供应商ID，VARCHAR类型，作为主键
    Sname VARCHAR(100) NOT NULL,                 -- 供应商名，不能为空
    telephone VARCHAR(20) NOT NULL,              -- 电话号码，不能为空
    email VARCHAR(100) NOT NULL,                 -- 电子邮件，不能为空
    province VARCHAR(50) NOT NULL,               -- 省，不能为空
    city VARCHAR(50) NOT NULL,                   -- 市，不能为空
    district VARCHAR(50) NOT NULL,               -- 区，不能为空
    detailed_address VARCHAR(255) NOT NULL,      -- 详细地址，不能为空
    contact_person VARCHAR(100) NOT NULL,        -- 联系人姓名，不能为空
    UNIQUE (telephone),                          -- 电话号码唯一约束
    CHECK (CHAR_LENGTH(Sid) >= 6)                -- 供应商ID长度必须大于6
);

--  创建Medicine表
CREATE TABLE Medicine (
    Mid VARCHAR(20)  PRIMARY KEY,
    Mname VARCHAR(100) NOT NULL,
    price DECIMAL(10, 2) NOT NULL,
    quantity INT NOT NULL,
    Sid VARCHAR(20) NOT NULL,
    details TEXT,
    FOREIGN KEY (Sid) REFERENCES Supplier(Sid) ON DELETE CASCADE
);

--  创建Order表
CREATE TABLE `Order` (
    `Oid` INT AUTO_INCREMENT PRIMARY KEY,
    `Cid` VARCHAR(20) NOT NULL,
    `Mid` VARCHAR(20) NOT NULL,
    `Sid` VARCHAR(20) NOT NULL,
    Mname VARCHAR(100) NOT NULL,  
    `number` INT NOT NULL,
    `time` TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    `price` DECIMAL(10, 2) NOT NULL,
    `province` VARCHAR(100) NOT NULL,
    `city` VARCHAR(100) NOT NULL,
    `district` VARCHAR(100) NOT NULL,
    `detailed_address` VARCHAR(255) NOT NULL,
    `state` ENUM('Pending payment', 'Pending delivery', 'Transitting delivery', 'Received delivery') NOT NULL,
    FOREIGN KEY (`Cid`) REFERENCES `Customer` (`Cid`) ON DELETE CASCADE,
    FOREIGN KEY (`Mid`) REFERENCES `Medicine` (`Mid`) ON DELETE CASCADE,
    FOREIGN KEY (`Sid`) REFERENCES `Supplier` (`Sid`) ON DELETE CASCADE
);                          

-- 触发器： 当订单下单时，对应的药品数量就要减少
DELIMITER //

CREATE TRIGGER update_medicine_quantity
AFTER INSERT ON `Order`
FOR EACH ROW
BEGIN
    UPDATE `Medicine`
    SET `quantity` = `quantity` - NEW.`number`
    WHERE `Mid` = NEW.`Mid`;
END //

DELIMITER ;

-- 触发器：当订单删除时，恢复对应的药品数量
DELIMITER //

CREATE TRIGGER restore_medicine_quantity
AFTER DELETE ON `Order`
FOR EACH ROW
BEGIN
    UPDATE `Medicine`
    SET `quantity` = `quantity` + OLD.`number`
    WHERE `Mid` = OLD.`Mid`;
END //

DELIMITER ;
