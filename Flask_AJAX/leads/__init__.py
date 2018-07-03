from flask import Flask
from flask_sqlalchemy import SQLAlchemy

app = Flask(__name__)

app.secret_key = "derp"


db = SQLAlchemy(app)

from leads import views

