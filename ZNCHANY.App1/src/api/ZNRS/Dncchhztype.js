

import axios from '@/libs/api.request'

export const getChhztypeListAll = () => {
  return axios.request({
    url:  'Dncchhztype' +'/list',
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/"
  })
}

export const getChhztypeList = (data) => {
  return axios.request({
    url:  'Dncchhztype' +'/list',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

// createRole
export const createChhztype = (data) => {
  return axios.request({
    url:  'Dncchhztype' +'/create',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

//loadRole
export const loadChhztype = (data) => {
  return axios.request({
    url: 'Dncchhztype' +'/edit/' + data.code,
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/"
  })
}

// editRole
export const editChhztype = (data) => {
  return axios.request({
    url: 'Dncchhztype' +'/edit',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

// delete role
export const deleteChhztype = (ids) => {
  return axios.request({
    url: 'Dncchhztype'+'/delete/' + ids,
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    method: 'get'
  })
}

// batch command
export const batchCommand = (data) => {
  return axios.request({
    url: 'Dncchhztype'+'/batch',
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    params: data
  })
}


export const batchCreateChhztype = (data) => {
  return axios.request({
    url:  'Dncchhztype' +'/batchcreate',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    params:data
  })
}

