

import axios from '@/libs/api.request'

export const getExpand3dListAll = () => {
  return axios.request({
    url:  'Dncexpand3d' +'/list',
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/"
  })
}

export const getExpand3dList = (data) => {
  return axios.request({
    url:  'Dncexpand3d' +'/list',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

// createRole
export const createExpand3d = (data) => {
  return axios.request({
    url:  'Dncexpand3d' +'/create',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

//loadRole
export const loadExpand3d = (data) => {
  return axios.request({
    url: 'Dncexpand3d' +'/edit/' + data.code,
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/"
  })
}

// editRole
export const editExpand3d = (data) => {
  return axios.request({
    url: 'Dncexpand3d' +'/edit',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

// delete role
export const deleteExpand3d = (ids) => {
  return axios.request({
    url: 'Dncexpand3d'+'/delete/' + ids,
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    method: 'get'
  })
}

// batch command
export const batchCommand = (data) => {
  return axios.request({
    url: 'Dncexpand3d'+'/batch',
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    params: data
  })
}


export const batchCreateExpand3d = (data) => {
  return axios.request({
    url:  'Dncexpand3d' +'/batchcreate',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    params:data
  })
}

