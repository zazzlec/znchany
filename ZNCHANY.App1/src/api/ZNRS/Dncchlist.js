

import axios from '@/libs/api.request'

export const getChlistListAll = () => {
  return axios.request({
    url:  'Dncchlist' +'/list',
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/"
  })
}

export const getChlistList = (data) => {
  return axios.request({
    url:  'Dncchlist' +'/list',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

// createRole
export const createChlist = (data) => {
  return axios.request({
    url:  'Dncchlist' +'/create',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

//loadRole
export const loadChlist = (data) => {
  return axios.request({
    url: 'Dncchlist' +'/edit/' + data.code,
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/"
  })
}

export const runpush = (data) => {
  return axios.request({
    url: 'Dncchlist' +'/RunPush/?bid=' + data.bid+'&type='+data.type,
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/"
  })
}

// editRole
export const editChlist = (data) => {
  return axios.request({
    url: 'Dncchlist' +'/edit',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

// delete role
export const deleteChlist = (ids) => {
  return axios.request({
    url: 'Dncchlist'+'/delete/' + ids,
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    method: 'get'
  })
}

// batch command
export const batchCommand = (data) => {
  return axios.request({
    url: 'Dncchlist'+'/batch',
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    params: data
  })
}


export const batchCreateChlist = (data) => {
  return axios.request({
    url:  'Dncchlist' +'/batchcreate',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    params:data
  })
}

