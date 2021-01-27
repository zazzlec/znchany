

import axios from '@/libs/api.request'

export const getError_parameterListAll = () => {
  return axios.request({
    url:  'Dncerror_parameter' +'/list',
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/"
  })
}

export const getError_parameterList = (data) => {
  return axios.request({
    url:  'Dncerror_parameter' +'/list',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

// createRole
export const createError_parameter = (data) => {
  return axios.request({
    url:  'Dncerror_parameter' +'/create',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

//loadRole
export const loadError_parameter = (data) => {
  return axios.request({
    url: 'Dncerror_parameter' +'/edit/' + data.code,
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/"
  })
}

// editRole
export const editError_parameter = (data) => {
  return axios.request({
    url: 'Dncerror_parameter' +'/edit',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    data
  })
}

// delete role
export const deleteError_parameter = (ids) => {
  return axios.request({
    url: 'Dncerror_parameter'+'/delete/' + ids,
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    method: 'get'
  })
}

// batch command
export const batchCommand = (data) => {
  return axios.request({
    url: 'Dncerror_parameter'+'/batch',
    method: 'get',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    params: data
  })
}


export const batchCreateError_parameter = (data) => {
  return axios.request({
    url:  'Dncerror_parameter' +'/batchcreate',
    method: 'post',
    withPrefix: false,
    prefix:"api/ZNCHANY1/",
    params:data
  })
}

