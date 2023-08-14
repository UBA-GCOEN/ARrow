import userStudentModel from "../models/userStudentModel.js"


/**
 * Route: /userStudent
 * Desc: to show or Access user Student
 */
export const userStudent = async (req, res) => {
    res.status(200).json({message:"Show user student signin/signup page"})

}


/**
 * Route: /userStudent/signup
 * Desc: Student user sign up
 */
export const signup = async (req, res) => {
       const { name, enrollNo, email, password} = req.body

       const oldUser = await userStudentModel.findOne({ email });
      try{
        if(!oldUser){
            const result = userStudentModel.create({
                name,
                enrollNo,
                email,
                password,
             });
    
             if(result){
                res.json({msg: "user Student added successfully"})
             }
           }
           else{
            res.json({msg: "user already exist"})
           }
      }
      catch(err){
        console.log(err)
      }
     
   

}