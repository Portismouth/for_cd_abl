from flask import Flask, request, redirect, render_template, session, flash, jsonify
from mysqlconnection import MySQLConnector
import re
import md5
import os
import binascii
import math
app = Flask(__name__)
EMAIL_REGEX = re.compile(r'^[a-zA-Z0-9.+_-]+@[a-zA-Z0-9._-]+\.[a-zA-Z]+$')
mysql = MySQLConnector(app, 'ajax_db')
app.secret_key = "derp"


@app.route('/')
def index():
    data_total = mysql.query_db("SELECT COUNT(*) FROM data")
    per_page = 4
    num_records = data_total[0]['COUNT(*)']
    pages = math.ceil(num_records / per_page) + 1
    page_links = nav_list(int(pages))
    data = mysql.query_db(
      "SELECT * FROM data LIMIT " + str(per_page)
    )
    return render_template('index.html', data=data, pages=page_links)

def nav_list(pages):
    page_numbers = []
    for x in range(int(pages)):
      page_numbers.append(x + 1)
    return page_numbers

@app.route('/process', methods=['POST'])
def process():
    query = "INSERT INTO data (data) values(:data)"
    data = {
        'data': request.form['data']
    }
    mysql.query_db(query, data)
    response = mysql.query_db("SELECT * from data")
    return jsonify(response)


@app.route('/find', methods=['POST'])
def find():
    print('here')
    search = request.form['starts_with']
    per_page = 4
    if request.POST.get('page_num'):
      page_number = request.POST.get('page_num')
    else:
      page_number = 1

    page = int(page_number)
    current_page = page
    page -= 1
    start = page * per_page
    if search:
      data = mysql.query_db("SELECT * FROM data WHERE data CONTAINS " + search + "LIMIT " + per_page + " OFFSET " + start)
      count = mysql.query_db("SELECT COUNT(*) FROM data WHERE data CONTAINS " + search)
    else:
      data = mysql.query_db(
      "SELECT * FROM data LIMIT " + str(per_page) + " OFFSET " + start
      )
      count = mysql.query_db("SELECT COUNT(*) FROM data")

    return render_template("data.html", data=data, count=count[0]['COUNT(*)'])

app.run(debug=True)
