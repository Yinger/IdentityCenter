import originAxios from "axios";
import { message } from "antd";

const axios = originAxios.create({
  timeout: 20000,
});

axios.interceptors.response.use(
  function (response) {
    if (response.data && !response.data.success) {
      let errorMsg = response.data.msg;
      message.error(errorMsg);
      return Promise.reject(errorMsg);
    }
    return response.data;
  },
  function (error) {
    return Promise.reject(error);
  }
);

export function get(url: string, data: any) {
  return axios.get(url, {
    params: data,
  });
}

// By default, axios serializes JavaScript objects to JSON.
export function post(url: string, data: any) {
  // console.log(data);
  return axios({
    method: "post",
    url,
    data,
  });
}

export function put(url: string, data: any) {
  return axios({
    method: "put",
    url,
    data,
  });
}

export function remove(url: string) {
  return axios({
    method: "delete",
    url,
  });
}

export default axios;
