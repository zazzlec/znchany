

import axios from '@/libs/api.request'

export const getChhzpointnowListAll = () => {
  return axios.request({
    url:  'Dncchhzpointnow' +'/list',
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/"
  })
}

export const getChhzpointnowList = (data) => {
  return axios.request({
    url:  'Dncchhzpointnow' +'/list',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

// createRole
export const createChhzpointnow = (data) => {
  return axios.request({
    url:  'Dncchhzpointnow' +'/create',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

//loadRole
export const loadChhzpointnow = (data) => {
  return axios.request({
    url: 'Dncchhzpointnow' +'/edit/' + data.code,
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/"
  })
}

// editRole
export const editChhzpointnow = (data) => {
  return axios.request({
    url: 'Dncchhzpointnow' +'/edit',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

// delete role
export const deleteChhzpointnow = (ids) => {
  return axios.request({
    url: 'Dncchhzpointnow'+'/delete/' + ids,
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    method: 'get'
  })
}

// batch command
export const batchCommand = (data) => {
  return axios.request({
    url: 'Dncchhzpointnow'+'/batch',
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    params: data
  })
}


export const batchCreateChhzpointnow = (data) => {
  return axios.request({
    url:  'Dncchhzpointnow' +'/batchcreate',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    params:data
  })
}

