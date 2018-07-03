import datetime
from leads import db

class Lead(db.Model):
    __tablename__ = 'leads'

    id = db.Column(db.Integer, primary_key=True)
    first_name = db.Column(db.String(20), nullable=False)
    last_name = db.Column(db.String(40), nullable=False)
    email = db.Column(db.String(100), nullable=False)
    created_at = db.Column(db.DateTime, nullable=False, default=datetime.datetime.utcnow)

    def __repr__(self):
        return "<Lead(first_name='%s', last_name='%s', email='%s', created_at='%s')>" % (self.first_name, self.last_name, self.email, self.created_at)
