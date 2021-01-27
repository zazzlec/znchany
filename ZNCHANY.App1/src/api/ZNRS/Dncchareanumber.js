

import axios from '@/libs/api.request'

export const getChareanumberListAll = () => {
  return axios.request({
    url:  'Dncchareanumber' +'/list',
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/"
  })
}

export const getChareanumberList = (data) => {
  return axios.request({
    url:  'Dncchareanumber' +'/list',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

// createRole
export const createChareanumber = (data) => {
  return axios.request({
    url:  'Dncchareanumber' +'/create',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

//loadRole
export const loadChareanumber = (data) => {
  return axios.request({
    url: 'Dncchareanumber' +'/edit/' + data.code,
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/"
  })
}

// editRole
export const editChareanumber = (data) => {
  return axios.request({
    url: 'Dncchareanumber' +'/edit',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

// delete role
export const deleteChareanumber = (ids) => {
  return axios.request({
    url: 'Dncchareanumber'+'/delete/' + ids,
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    method: 'get'
  })
}

// batch command
export const batchCommand = (data) => {
  return axios.request({
    url: 'Dncchareanumber'+'/batch',
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    params: data
  })
}


export const batchCreateChareanumber = (data) => {
  return axios.request({
    url:  'Dncchareanumber' +'/batchcreate',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    params:data
  })
}

