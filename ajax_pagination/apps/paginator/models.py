# -*- coding: utf-8 -*-
from __future__ import unicode_literals

from django.db import models

# Create your models here.
class Message(models.Model):
  message = models.TextField()
class Lead(models.Model):
  first_name = models.TextField()
  last_name = models.TextField()
  registered = models.DateField(auto_now_add=True)