

import axios from '@/libs/api.request'

export const getKyqpointdataListAll = () => {
  return axios.request({
    url:  'Dnckyqpointdata' +'/list',
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/"
  })
}

export const getKyqpointdataList = (data) => {
  return axios.request({
    url:  'Dnckyqpointdata' +'/list',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

// createRole
export const createKyqpointdata = (data) => {
  return axios.request({
    url:  'Dnckyqpointdata' +'/create',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

//loadRole
export const loadKyqpointdata = (data) => {
  return axios.request({
    url: 'Dnckyqpointdata' +'/edit/' + data.code,
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/"
  })
}

// editRole
export const editKyqpointdata = (data) => {
  return axios.request({
    url: 'Dnckyqpointdata' +'/edit',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

// delete role
export const deleteKyqpointdata = (ids) => {
  return axios.request({
    url: 'Dnckyqpointdata'+'/delete/' + ids,
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    method: 'get'
  })
}

// batch command
export const batchCommand = (data) => {
  return axios.request({
    url: 'Dnckyqpointdata'+'/batch',
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    params: data
  })
}


export const batchCreateKyqpointdata = (data) => {
  return axios.request({
    url:  'Dnckyqpointdata' +'/batchcreate',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    params:data
  })
}

