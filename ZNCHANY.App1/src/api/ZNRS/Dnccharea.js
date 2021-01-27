

import axios from '@/libs/api.request'

export const getChareaListAll = () => {
  return axios.request({
    url:  'Dnccharea' +'/list',
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/"
  })
}

export const getChareaList = (data) => {
  return axios.request({
    url:  'Dnccharea' +'/list',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

// createRole
export const createCharea = (data) => {
  return axios.request({
    url:  'Dnccharea' +'/create',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

//loadRole
export const loadCharea = (data) => {
  return axios.request({
    url: 'Dnccharea' +'/edit/' + data.code,
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/"
  })
}

// editRole
export const editCharea = (data) => {
  return axios.request({
    url: 'Dnccharea' +'/edit',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

// delete role
export const deleteCharea = (ids) => {
  return axios.request({
    url: 'Dnccharea'+'/delete/' + ids,
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    method: 'get'
  })
}

// batch command
export const batchCommand = (data) => {
  return axios.request({
    url: 'Dnccharea'+'/batch',
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    params: data
  })
}


export const batchCreateCharea = (data) => {
  return axios.request({
    url:  'Dnccharea' +'/batchcreate',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    params:data
  })
}

