# -*- coding: utf-8 -*-
from __future__ import unicode_literals

from django.shortcuts import render, HttpResponse, redirect
from models import Message, Lead
from django.core import serializers
import json
import math

# Create your views here.
def index(request):
  context = {
    'greeting': "Welcome to the Paginator!!!",
    'leads': Lead.objects.all()
  }
  leads = Lead.objects.all()
  return render(request, 'index.html', context)

def leads(request):
  context = {
    'leads': Lead.objects.all()
  }
  return render(request, 'leads.html', context)

def process(request):
  if request.method == "POST":
    print(request.POST)
    Lead.objects.create(
      first_name=request.POST['first_name'],
      last_name=request.POST['last_name']
    )
    leads = Lead.objects.all()
    
  return HttpResponse(serializers.serialize('json', leads), content_type='application/json')

def find(request):
  if request.method == "POST":
    print(request.POST)
    search = request.POST.get('starts_with')
    per_page = 4
    if request.POST.get('page_num'):
      page_number = request.POST.get('page_num')
    else:
      page_number = 1

    page = int(page_number)

    current_page = page
    page -= 1
    start = page * per_page
    print(start)

    if search:
      leads = Lead.objects.filter(first_name__startswith=request.POST['starts_with'])[start:per_page]
      count = Lead.objects.filter(first_name__startswith=request.POST['starts_with']).count()
    else:
      leads = Lead.objects.all()[start:current_page * per_page]
      count = Lead.objects.all().count()
    
    pages = math.ceil(count / per_page) + 1
    context = {
      'pages': navigation_list(pages),
      'leads': leads
    }
  return render(request, 'leads.html', context)

def navigation_list(pages):
  page_numbers = []
  for x in range(int(pages)):
    page_numbers.append(x + 1)
  return page_numbers
