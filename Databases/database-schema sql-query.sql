/*----city----*/
CREATE TABLE city(
	postalcode int NOT NULL PRIMARY KEY,
	city_name varchar(30) NOT NULL
);

/*------address----*/
CREATE TABLE [address](
	add_id int NOT NULL PRIMARY KEY,
	street varchar(50) NOT NULL,
	house_no varchar(30) NOT NULL,
	postalcode int NOT NULL FOREIGN KEY REFERENCES city(postalcode)
);

/*------customer-----*/
CREATE TABLE customer(
	cust_id int NOT NULL PRIMARY KEY,
	first_name varchar(30) NOT NULL,
	last_name varchar(30),
	email_id varchar(30) NOT NULL UNIQUE,
	mobile_no int NOT NULL,
	[password] varchar(30) NOT NULL,
	add_id int FOREIGN KEY REFERENCES address(add_id)
);

/*------helper-----*/
CREATE TABLE helper(
	helper_id int NOT NULL PRIMARY KEY,
	first_name varchar(30) NOT NULL,
	last_name varchar(30),
	email_id varchar(30) NOT NULL UNIQUE,
	mobile_no int NOT NULL,
	[password] varchar(30) NOT NULL,
	add_id int FOREIGN KEY REFERENCES address(add_id)
);

/*--------payment------*/
CREATE TABLE payment(
	order_id int NOT NULL PRIMARY KEY,
	transaction_id int NOT NULL,
	card_num int NOT NULL,
	CVV int NOT NULL,
	amount float NOT NULL,
	promocode varchar(30),
	remarks varchar(150)
);

/*------refund-------*/
CREATE TABLE refund(
	invoice_id int NOT NULL PRIMARY KEY,
	refund_amount float NOT NULL,
	[description] varchar(150)
);

/*-------invoice-----*/
CREATE TABLE invoice (
    cust_id int NOT NULL PRIMARY KEY,
	order_id int NOT NULL FOREIGN KEY REFERENCES payment(order_id),
	invoice_id int NOT NULL FOREIGN KEY REFERENCES refund(invoice_id),
);

/*---------bookedservice-----*/
CREATE TABLE bookedservice(
	cust_id int NOT NULL PRIMARY KEY,
	order_id int NOT NULL FOREIGN KEY REFERENCES payment(order_id),
	service_date date NOT NULL,
	service_time time NOT NULL,
	service_hours time NOT NULL,
	add_id int FOREIGN KEY REFERENCES address(add_id),
	comments varchar(150),
	pets bit,
	accept bit
);

/*-------cancel-----*/
CREATE TABLE cancel(
	order_id int NOT NULL PRIMARY KEY,
	[description] varchar(150) 
);

/*-------reschedule-----*/
CREATE TABLE reschedule(
	order_id int NOT NULL PRIMARY KEY,
	new_date date NOT NULL,
	new_time time NOT NULL,
	[description] varchar(150)
);

/*-------login-----*/
CREATE TABLE [login](
	email_id varchar(30) NOT NULL,
	[password] varchar(30) NOT NULL
);

/*------favourite-blocked-----*/
CREATE TABLE favourite_blocked(
	cust_id int NOT NULL PRIMARY KEY,
	order_id int NOT NULL FOREIGN KEY REFERENCES payment(order_id),
	is_favourite bit,
	is_blocked bit
);

/*------newsletter-----*/
CREATE TABLE newsletter (
	email_id varchar(30) NOT NULL
);

/*--------getintouch------*/
CREATE TABLE getintouch (
    first_name varchar(30) NOT NULL,
	last_name varchar(30),
	email_id varchar(30) NOT NULL,
	[message] varchar(150) NOT NULL,
);
