from flask import request, redirect, render_template, flash, jsonify
from leads import app
import math
from leads.models import Lead
from mysqlconnection import MySQLConnector
from leads import db

mysql = MySQLConnector(app, 'ajax_db')

# set page limit
results_per_page = 4
# Gets total number of records from db
data_total = Lead.query.all()
# parses number of records from db query
num_records = len(data_total)

print num_records

@app.route('/')
def index():
    # calculates number of pages
    pages = math.ceil(num_records / results_per_page) + 1
    print pages
    # creates page links using helper method below
    page_links = nav_list(int(pages))
    # db query limiting number of results per page
    leads = Lead.query.limit(results_per_page).all()
    return render_template('index.html', pages=page_links, current_page=1, leads=leads)


@app.route('/create', methods=['POST'])
def process():
    # calculates number of pages
    pages = math.ceil(num_records / results_per_page) + 1
    # creates page links using helper method below
    page_links = nav_list(int(pages))
    # db query limiting number of results per page
    lead = Lead(
        first_name=request.form['first_name'],
        last_name=request.form['last_name'],
        email=request.form['email'],
    )
    db.session.add(lead)
    db.session.commit()
    leads = Lead.query.limit(results_per_page).all()
    return render_template('/includes/_data.html', pages=page_links, current_page=1, leads=leads, all_results_pages=nav_list(int(pages)))


@app.route('/find', methods=['POST'])
def find():
    if request.form['page_num']:
        page_number = request.form['page_num']
    else:
        page_number = 1

    page = int(page_number)
    current_page = page
    page -= 1
    start = page * results_per_page

    data = mysql.query_db(
        "SELECT * FROM leads LIMIT " +
        str(results_per_page) + " OFFSET " + str(start)
    )

    pages = math.ceil(num_records / results_per_page) + 1

    return render_template("/includes/_data.html", leads=data, all_results_pages=nav_list(int(pages)), current_page=current_page, search_results_pages=0)


@app.route('/search', methods=['POST'])
def search():
    search = request.form['search']
    if(search == ''):
        search_results = mysql.query_db(
            "SELECT * FROM leads LIMIT " + str(results_per_page)
        )
        pages = math.ceil(num_records / results_per_page) + 1
    else:
        search_results = mysql.query_db(
            "SELECT * FROM leads WHERE first_name LIKE '%" +
            (search + "%' LIMIT " + str(results_per_page))
        )
        pages = math.ceil(len(search_results) / results_per_page) + 1

    return render_template("/includes/_data.html", leads=search_results, search_results_pages=nav_list(pages), search_term=search, current_page=1, all_results_pages=0)


@app.route('/find-search', methods=['POST'])
def find_search():
    if request.form['page_num']:
        page_number = request.form['page_num']
    else:
        page_number = 1

    page = int(page_number)
    current_page = page
    page -= 1
    start = page * results_per_page

    if request.form['search_term']:
        search_term = request.form['search_term']
        total_matching_results = mysql.query_db(
            "SELECT COUNT(*) FROM leads WHERE first_name LIKE '%" + search_term + "%'"
        )
        data = mysql.query_db(
            "SELECT * FROM leads WHERE first_name LIKE '%" +
            (search_term) + "%' LIMIT " +
            str(results_per_page) + " OFFSET " + str(start)
        )
        count = int(total_matching_results[0]['COUNT(*)'])
    else:
        search_term = ''
        data = mysql.query_db(
            "SELECT * FROM leads LIMIT " +
            str(results_per_page) + " OFFSET " + str(start)
        )
        count = num_records

    pages = math.ceil(count / results_per_page) + 1

    return render_template("/includes/_data.html", leads=data, search_results_pages=nav_list(int(pages)), current_page=current_page, search_term=search_term, all_results_pages=0)


def nav_list(pages):
    page_numbers = []
    for x in range(int(pages)):
      page_numbers.append(x + 1)
    return page_numbers
