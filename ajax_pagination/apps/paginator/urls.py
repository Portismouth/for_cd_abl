from django.conf.urls import url
from . import views

urlpatterns = [
  url(r'^$', views.index),
  url(r'^process$', views.process),
  # url(r'^leads$', views.leads),
  url(r'^find$', views.find)
]