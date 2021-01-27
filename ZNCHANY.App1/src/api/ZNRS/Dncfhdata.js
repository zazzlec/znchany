

import axios from '@/libs/api.request'

export const getFhdataListAll = () => {
  return axios.request({
    url:  'Dncfhdata' +'/list',
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/"
  })
}

export const getFhdataList = (data) => {
  return axios.request({
    url:  'Dncfhdata' +'/list',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

// createRole
export const createFhdata = (data) => {
  return axios.request({
    url:  'Dncfhdata' +'/create',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

//loadRole
export const loadFhdata = (data) => {
  return axios.request({
    url: 'Dncfhdata' +'/edit/' + data.code,
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/"
  })
}

// editRole
export const editFhdata = (data) => {
  return axios.request({
    url: 'Dncfhdata' +'/edit',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

// delete role
export const deleteFhdata = (ids) => {
  return axios.request({
    url: 'Dncfhdata'+'/delete/' + ids,
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    method: 'get'
  })
}

// batch command
export const batchCommand = (data) => {
  return axios.request({
    url: 'Dncfhdata'+'/batch',
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    params: data
  })
}


export const batchCreateFhdata = (data) => {
  return axios.request({
    url:  'Dncfhdata' +'/batchcreate',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    params:data
  })
}

