

import axios from '@/libs/api.request'

export const getChqpointdataListAll = () => {
  return axios.request({
    url:  'Dncchqpointdata' +'/list',
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/"
  })
}

export const getChqpointdataList = (data) => {
  return axios.request({
    url:  'Dncchqpointdata' +'/list',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

// createRole
export const createChqpointdata = (data) => {
  return axios.request({
    url:  'Dncchqpointdata' +'/create',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

//loadRole
export const loadChqpointdata = (data) => {
  return axios.request({
    url: 'Dncchqpointdata' +'/edit/' + data.code,
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/"
  })
}

// editRole
export const editChqpointdata = (data) => {
  return axios.request({
    url: 'Dncchqpointdata' +'/edit',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

// delete role
export const deleteChqpointdata = (ids) => {
  return axios.request({
    url: 'Dncchqpointdata'+'/delete/' + ids,
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    method: 'get'
  })
}

// batch command
export const batchCommand = (data) => {
  return axios.request({
    url: 'Dncchqpointdata'+'/batch',
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    params: data
  })
}


export const batchCreateChqpointdata = (data) => {
  return axios.request({
    url:  'Dncchqpointdata' +'/batchcreate',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    params:data
  })
}

