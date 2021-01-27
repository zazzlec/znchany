

import axios from '@/libs/api.request'

export const getKyqconfigListAll = () => {
  return axios.request({
    url:  'Dnckyqconfig' +'/list',
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/"
  })
}

export const getKyqconfigList = (data) => {
  return axios.request({
    url:  'Dnckyqconfig' +'/list',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

// createRole
export const createKyqconfig = (data) => {
  return axios.request({
    url:  'Dnckyqconfig' +'/create',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

//loadRole
export const loadKyqconfig = (data) => {
  return axios.request({
    url: 'Dnckyqconfig' +'/edit/' + data.code,
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/"
  })
}

// editRole
export const editKyqconfig = (data) => {
  return axios.request({
    url: 'Dnckyqconfig' +'/edit',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

// delete role
export const deleteKyqconfig = (ids) => {
  return axios.request({
    url: 'Dnckyqconfig'+'/delete/' + ids,
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    method: 'get'
  })
}

// batch command
export const batchCommand = (data) => {
  return axios.request({
    url: 'Dnckyqconfig'+'/batch',
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    params: data
  })
}


export const batchCreateKyqconfig = (data) => {
  return axios.request({
    url:  'Dnckyqconfig' +'/batchcreate',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    params:data
  })
}

