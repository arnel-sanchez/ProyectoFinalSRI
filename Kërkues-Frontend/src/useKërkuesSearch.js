import { useState, useEffect } from "react";

import Axios from "axios";

const useKërkuesSearch = (term) => {
  const [data_, setData] = useState(null);

  //const fetchDData = () => {Axios.post("https://localhost:7290/api/Search", {"search": term}).then(response => {console.log(response.data);})}
  //fetchDData();

  useEffect(() => {
    const fetchData = async () => {
      /*fetch("https://localhost:7290/api/Search", {"search": term})*/
      Axios.post("https://localhost:7290/api/Search", {"search": term})
        .then(response => response.data)
		.then((result) => {setData(result);});
    };

    fetchData()
      .then((res) => {
        console.log(res);
      })
      .catch((error) => {
        console.log(error);
      });
  }, [term]);

  console.log(data_);
  return data_;
};

export default useKërkuesSearch;
