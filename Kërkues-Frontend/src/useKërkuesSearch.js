import { useState, useEffect } from "react";

const useKërkuesSearch = (term) => {
  const [data, setData] = useState(null);

  useEffect(() => {
    const fetchData = async () => {
      fetch(
        
      )
        .then((response) => response.json())
        .then((result) => {
          setData(result);
        });
    };

    fetchData()
      .then((res) => {
        console.log(res);
      })
      .catch((error) => {
        console.log(error);
      });
  }, [term]);

  return { data };
};

export default useKërkuesSearch;
